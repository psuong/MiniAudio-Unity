using System.Text;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MiniAudio.Entities.Demo {

    public struct TrackedTag : ISystemStateComponentData { }

    [DisableAutoCreation]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class AudioDrawingSystem : SystemBase {

        static readonly StringBuilder StringBuilder = new StringBuilder(256);

        [BurstCompile]
        struct QueryAudioClipJob : IJobEntityBatch {

            [ReadOnly]
            public ComponentTypeHandle<TrackedTag> TrackedTagType;

            [ReadOnly]
            public ComponentTypeHandle<AudioClip> AudioClipType;

            [ReadOnly]
            public EntityTypeHandle EntityType;

            [WriteOnly]
            public NativeList<Entity> TrackedEntities;

            public EntityCommandBuffer CommandBuffer;

            public void Execute(ArchetypeChunk batchInChunk, int batchIndex) {
                var audioClips = batchInChunk.GetNativeArray(AudioClipType);
                var entities = batchInChunk.GetNativeArray(EntityType);

                if (!batchInChunk.Has(TrackedTagType)) {
                    for (int i = 0; i < batchInChunk.Count; i++) {
                        var clip = audioClips[i];
                        var entity = entities[i];

                        CommandBuffer.AddComponent<TrackedTag>(entity);
                        TrackedEntities.Add(entity);
                    }
                }
            }
        }

        EntityQuery audioQuery;
        EntityQuery destroyedAudioQuery;
        EntityCommandBufferSystem commandBufferSystem;
        NativeList<Entity> trackedEntities;

        protected override void OnCreate() {
            audioQuery = GetEntityQuery(new EntityQueryDesc {
                All = new[] {
                    ComponentType.ReadOnly<AudioClip>()
                }
            });

            destroyedAudioQuery = GetEntityQuery(new EntityQueryDesc {
                All = new[] {
                    ComponentType.ReadOnly<TrackedTag>()
                },
                None = new[] {
                    ComponentType.ReadOnly<AudioClip>()
                }
            });

            trackedEntities = new NativeList<Entity>(10, Allocator.Persistent);
        }

        protected override void OnDestroy() {
            if (trackedEntities.IsCreated) {
                trackedEntities.Dispose();
            }
        }

        protected override void OnUpdate() {
            if (AudioClipUi.Instance != null &&
                AudioClipUi.Instance.Containers.Count != trackedEntities.Length) {
                var audioClips = GetComponentDataFromEntity<AudioClip>(true);
                for (int i = 0; i < trackedEntities.Length; i++) {
                    var entity = trackedEntities[i];
                    
                }
            }

            new QueryAudioClipJob {
                TrackedTagType = GetComponentTypeHandle<TrackedTag>(true),
                AudioClipType = GetComponentTypeHandle<AudioClip>(true),
                TrackedEntities = trackedEntities,
            }.Run(audioQuery);
        }
    }
}
