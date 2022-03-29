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
        struct ManageAudioJob : IJobEntityBatch {

            [ReadOnly]
            public ComponentTypeHandle<AudioStateHistory> AudioStateHistoryType;

            [ReadOnly]
            public ComponentTypeHandle<AudioClip> AudioClipType;

            [ReadOnly]
            public EntityTypeHandle EntityType;

            public EntityCommandBuffer CommandBuffer;

            public uint LastSystemVersion;

            public void Execute(ArchetypeChunk batchInChunk, int batchIndex) {
                var audioClips = batchInChunk.GetNativeArray(AudioClipType);
                var stateTypes = batchInChunk.GetNativeArray(AudioStateHistoryType);
                var entities = batchInChunk.GetNativeArray(EntityType);

                for (int i = 0; i < batchInChunk.Count; i++) {
                    var audioClip = audioClips[i];
                    var lastState = stateTypes[i].Value;
                    var entity = entities[i];

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
                    } else if (audioClip.CurrentState == AudioState.Playing) {
                        bool isPlaying = MiniAudioHandler.IsSoundPlaying(audioClip.Handle);
                        if (!isPlaying) {
                            audioClip.CurrentState = AudioState.Stopped;
                            CommandBuffer.SetComponent(entity, audioClip);
                        }
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

            new ManageAudioJob() {
                AudioStateHistoryType = GetComponentTypeHandle<AudioStateHistory>(true),
                AudioClipType         = GetComponentTypeHandle<AudioClip>(true),
                CommandBuffer         = commandBuffer,
                LastSystemVersion     = LastSystemVersion,
                EntityType            = GetEntityTypeHandle()
            }.Run(soundQuery);
        }
    }
}
