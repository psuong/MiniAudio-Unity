using MiniAudio.Interop;
using Unity.Collections;
using Unity.Entities;

#if !UNITY_EDITOR
using Unity.Burst;
#endif

namespace MiniAudio.Entities.Systems {

    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class AudioSystem : SystemBase {

#if !UNITY_EDITOR
        [BurstCompile]
#endif
        unsafe struct LoadSoundJob : IJobEntityBatch {

            [ReadOnly]
            public BufferTypeHandle<LoadPath> LoadPathType;

            [ReadOnly]
            public EntityTypeHandle EntityType;

            [ReadOnly]
            public ComponentTypeHandle<AudioClip> AudioClipType;

            public EntityCommandBuffer CommandBuffer;

            public void Execute(ArchetypeChunk batchInChunk, int batchIndex) {
                var loadParams = batchInChunk.GetBufferAccessor(LoadPathType);
                var audioClips = batchInChunk.GetNativeArray(AudioClipType);
                var entities = batchInChunk.GetNativeArray(EntityType);

                for (int i = 0; i < batchInChunk.Count; i++) {
                    var entity = entities[i];
                    var audioClip = audioClips[i];
                    var pathBuffer = loadParams[i];

                    char* path = (char*)pathBuffer.GetUnsafeReadOnlyPtr();

                    var handle = MiniAudioHandler.UnsafeLoadSound(
                        path,
                        (uint)pathBuffer.Length,
                        audioClip.Parameters);

                    if (handle != uint.MaxValue) {
                        audioClip.Handle = handle;
                        CommandBuffer.SetComponent(entity, audioClip);
                        CommandBuffer.RemoveComponent<LoadPath>(entity);
                    }
                }
            }
        }

#if !UNITY_EDITOR
        [BurstCompile]
#endif
        struct StopSoundJob : IJobEntityBatch {

            [ReadOnly]
            public ComponentTypeHandle<AudioStateHistory> AudioStateHistoryType;

            [ReadOnly]
            public ComponentTypeHandle<AudioClip> AudioClipType;

            [ReadOnly]
            public EntityTypeHandle EntityType;

            public EntityCommandBuffer CommandBuffer;

            public void Execute(ArchetypeChunk batchInChunk, int batchIndex) {
                var audioClips = batchInChunk.GetNativeArray(AudioClipType);
                var stateTypes = batchInChunk.GetNativeArray(AudioStateHistoryType);
                var entities = batchInChunk.GetNativeArray(EntityType);

                for (int i = 0; i < batchInChunk.Count; i++) {
                    var audioClip = audioClips[i];
                    var lastState = stateTypes[i].Value;
                    var entity = entities[i];

                    // This should only check if the entity's sound has stopped playing.
                    switch (audioClip.CurrentState) {
                        case AudioState.Playing:
                            if (MiniAudioHandler.IsSoundFinished(audioClip.Handle)) {
                                audioClip.CurrentState = AudioState.Stopped;
                                CommandBuffer.SetComponent(entity, audioClip);
                            }
                            break;
                    }
                }
            }
        }

#if !UNITY_EDITOR
        [BurstCompile]
#endif
        [WithChangeFilter(typeof(AudioClip))]
        struct ManageAudioStateJob : IJobEntityBatch {

            [ReadOnly]
            public ComponentTypeHandle<AudioStateHistory> AudioStateHistoryType;

            [ReadOnly]
            public ComponentTypeHandle<AudioClip> AudioClipType;

            [ReadOnly]
            public EntityTypeHandle EntityType;

            public uint LastSystemVersion;

            public EntityCommandBuffer CommandBuffer;

            public void Execute(ArchetypeChunk batchInChunk, int batchIndex) {
                if (!batchInChunk.DidChange(AudioClipType, LastSystemVersion)) {
                    return;
                }
                var audioClips = batchInChunk.GetNativeArray(AudioClipType);
                var stateTypes = batchInChunk.GetNativeArray(AudioStateHistoryType);
                var entities = batchInChunk.GetNativeArray(EntityType);

                for (int i = 0; i < batchInChunk.Count; i++) {
                    var audioClip = audioClips[i];
                    var lastState = stateTypes[i].Value;
                    var entity = entities[i];

                    MiniAudioHandler.SetSoundVolume(audioClip.Handle, audioClip.Parameters.Volume);
                    // UnityEngine.Debug.Log(audioClip.Parameters.Volume);

                    if (lastState != audioClip.CurrentState) {
                        switch (audioClip.CurrentState) {
                            case AudioState.Playing:
                                MiniAudioHandler.PlaySound(audioClip.Handle);
                                break;
                            case AudioState.Stopped:
                                MiniAudioHandler.StopSound(audioClip.Handle, true);
                                break;
                            case AudioState.Paused:
                                MiniAudioHandler.StopSound(audioClip.Handle, false);
                                break;
                        }
                        CommandBuffer.SetComponent(entity, new AudioStateHistory {
                            Value = audioClip.CurrentState
                        });
                    }
                }
            }
        }


        EntityQuery initializationQuery;
        EntityQuery soundQuery;
        EntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate() {
            initializationQuery = GetEntityQuery(new EntityQueryDesc {
                All = new[] { ComponentType.ReadOnly<LoadPath>(), ComponentType.ReadWrite<AudioClip>() }
            });

            soundQuery = GetEntityQuery(new EntityQueryDesc() {
                All = new[] {
                    ComponentType.ReadOnly<AudioClip>(), ComponentType.ReadOnly<AudioStateHistory>()
                },
                None = new[] {
                    ComponentType.ReadOnly<LoadPath>(),
                    // ComponentType.ReadOnly<StreamingPathMetadata>()
                }
            });

            commandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            var commandBuffer = commandBufferSystem.CreateCommandBuffer();
            new LoadSoundJob {
                LoadPathType = GetBufferTypeHandle<LoadPath>(true),
                AudioClipType = GetComponentTypeHandle<AudioClip>(true),
                EntityType = GetEntityTypeHandle(),
                CommandBuffer = commandBuffer
            }.Run(initializationQuery);

            new StopSoundJob {
                AudioStateHistoryType = GetComponentTypeHandle<AudioStateHistory>(true),
                AudioClipType = GetComponentTypeHandle<AudioClip>(true),
                CommandBuffer = commandBuffer,
                EntityType = GetEntityTypeHandle()
            }.Run(soundQuery);

            new ManageAudioStateJob() {
                AudioStateHistoryType = GetComponentTypeHandle<AudioStateHistory>(true),
                AudioClipType = GetComponentTypeHandle<AudioClip>(true),
                CommandBuffer = commandBuffer,
                LastSystemVersion = LastSystemVersion,
                EntityType = GetEntityTypeHandle()
            }.Run(soundQuery);
        }
    }
}
