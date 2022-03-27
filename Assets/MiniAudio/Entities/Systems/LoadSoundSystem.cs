using MiniAudio.Interop;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MiniAudio.Entities.Systems {

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class LoadSoundSystem : SystemBase {

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

                    var handle = MiniAudioHandler.LoadSound(
                        path,
                        (uint)pathBuffer.Length,
                        audioClip.Parameters);

                    if (handle != uint.MaxValue) {
                        audioClip.Handle = handle;
                        CommandBuffer.SetComponent(entity, audioClip);
                        CommandBuffer.RemoveComponent<LoadPath>(entity);
                        MiniAudioHandler.PlaySound(handle);
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
                AudioClipType = GetComponentTypeHandle<AudioClip>(true),
                EntityType    = GetEntityTypeHandle(),
                CommandBuffer = commandBufferSystem.CreateCommandBuffer()
            }.Run(loadSoundQuery);
        }
    }
}
