using System.Text;
using ImGuiNET;
using UImGui;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MiniAudio.Entities.Demo {

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class AudioDrawingSystem : SystemBase {

        static readonly StringBuilder StringBuilder = new StringBuilder(256);

        [BurstCompile]
        struct QueryAudioClipJob : IJobEntityBatch {

            [ReadOnly]
            public ComponentTypeHandle<AudioClip> AudioClipType;

            [ReadOnly]
            public EntityTypeHandle EntityType;

            [WriteOnly]
            public NativeList<Entity> TrackedEntities;

            public void Execute(ArchetypeChunk batchInChunk, int batchIndex) {
                var audioClips = batchInChunk.GetNativeArray(AudioClipType);
                var entities = batchInChunk.GetNativeArray(EntityType);

                for (int i = 0; i < batchInChunk.Count; i++) {
                    var clip = audioClips[i];
                    var entity = entities[i];
                    TrackedEntities.Add(entity);
                }
            }
        }

        EntityQuery audioQuery;
        EntityCommandBufferSystem commandBufferSystem;
        NativeList<Entity> trackedEntities;

        protected override void OnCreate() {
            audioQuery = GetEntityQuery(new EntityQueryDesc {
                All = new[] {
                    ComponentType.ReadOnly<AudioClip>()
                }
            });

            trackedEntities = new NativeList<Entity>(10, Allocator.Persistent);
            commandBufferSystem = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning() {
            UImGuiUtility.Layout += OnLayout;
        }

        protected override void OnStopRunning() {
            UImGuiUtility.Layout -= OnLayout;
        }

        protected override void OnDestroy() {
            if (trackedEntities.IsCreated) {
                trackedEntities.Dispose();
            }
        }

        protected override void OnUpdate() {
            new QueryAudioClipJob {
                AudioClipType = GetComponentTypeHandle<AudioClip>(true),
                EntityType = GetEntityTypeHandle(),
                TrackedEntities = trackedEntities,
            }.Run(audioQuery);
        }

        unsafe void OnLayout(UImGui.UImGui obj) {
            var audioClips = GetComponentDataFromEntity<AudioClip>(true);
            var loadPaths = GetComponentDataFromEntity<Path>(true);
            var commandBuffer = commandBufferSystem.CreateCommandBuffer();

            ImGui.Begin("MiniAudio Demo");
            for (int i = 0; i < trackedEntities.Length; i++) {
                var entity = trackedEntities[i];
                var loadPath = loadPaths[entity];
                var audioClip = audioClips[entity];

                ref var path = ref loadPath.Value.Value.Path;

                StringBuilder.Clear().Append("File: ");
                for (int j = 0; j < path.Length; j++) {
                    StringBuilder.Append(path[j]);
                }
                ImGui.Text(StringBuilder.ToString());

                bool changed = false;

                switch (audioClip.CurrentState) {
                    case AudioState.Paused:
                        if (ImGui.Button("Resume")) {
                            audioClip.CurrentState = AudioState.Playing;
                            changed |= true;
                        }
                        break;
                    case AudioState.Playing:
                        if (ImGui.Button("Pause")) {
                            audioClip.CurrentState = AudioState.Paused;
                            changed |= true;
                        }
                        break;
                    case AudioState.Stopped:
                        if (ImGui.Button("Play")) {
                            audioClip.CurrentState = AudioState.Playing;
                            changed |= true;
                        }
                        break;
                }

                ImGui.SameLine();
                if (ImGui.Button("Stop")) {
                    audioClip.CurrentState = AudioState.Stopped;
                    changed |= true;
                }

                var volume = math.sqrt(audioClip.Parameters.Volume);
                changed |= ImGui.SliderFloat("Volume", ref volume, 0, 1f);
                audioClip.Parameters.Volume = volume * volume;

                if (changed) {
                    commandBuffer.SetComponent(entity, audioClip);
                }
            }
            ImGui.End();

            trackedEntities.Clear();
        }
    }
}
