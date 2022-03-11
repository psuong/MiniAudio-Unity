using MiniAudio.Interop;
using Unity.Collections;
using Unity.Entities;

namespace MiniAudio.Entities.Systems {

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class LoadSoundSystem : SystemBase {

        unsafe struct LoadSoundJob : IJobEntityBatch {

            [ReadOnly]
            public BufferTypeHandle<LoadPath> LoadPathType;

            [ReadOnly]
            public EntityTypeHandle EntityType;

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

                    var handle = MiniAudioHandler.LoadSound(
                        path,
                        (uint)(pathBuffer.Length * sizeof(char)),
                        new SoundLoadParameters {
                            Volume = 1.0f
                        });

                    if (audioClip.State == AudioState.Stopped && handle != uint.MaxValue) {
                        audioClip.State = AudioState.Playing;
                        audioClip.Handle = handle;
                        CommandBuffer.RemoveComponent<LoadPath>(entity);
                        MiniAudioHandler.PlaySound(handle);
                        audioClips[i] = audioClip;
                    }
                }
            }
        }

        EntityQuery loadSoundQuery;
        EntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate() {
            loadSoundQuery = GetEntityQuery(new EntityQueryDesc {
                All = new[] { ComponentType.ReadOnly<LoadPath>(), ComponentType.ReadWrite<AudioClip>() }
            });

            commandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            new LoadSoundJob {
                LoadPathType  = GetBufferTypeHandle<LoadPath>(true),
                EntityType    = GetEntityTypeHandle(),
                AudioClipType = GetComponentTypeHandle<AudioClip>(false),
                CommandBuffer = commandBufferSystem.CreateCommandBuffer()
            }.Run(loadSoundQuery);
        }
    }
}
