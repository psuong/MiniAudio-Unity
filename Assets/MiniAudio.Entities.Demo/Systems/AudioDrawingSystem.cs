using System.Text;
using ImGuiNET;
using MiniAudio.Entities.Systems;
using UImGui;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MiniAudio.Entities.Demo {

    [UpdateInGroup(typeof(SimulationSystemGroup))]
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

        [BurstCompile]
        struct QueryPooledAudioJob : IJobEntityBatch {

            [ReadOnly]
            public EntityTypeHandle EntityType;

            [WriteOnly]
            public NativeList<Entity> TrackedEntities;

            public void Execute(ArchetypeChunk batchInChunk, int batchIndex) {
                var entities = batchInChunk.GetNativeArray(EntityType);
                for (int i = 0; i < batchInChunk.Count; i++) {
                    var entity = entities[i];
                    TrackedEntities.Add(entity);
                }
            }
        }

        EntityQuery audioQuery;
        EntityQuery pooledAudioQuery;

        OneShotAudioSystem oneShotAudioSystem;
        EntityCommandBufferSystem commandBufferSystem;
        NativeList<Entity> trackedEntities;
        NativeList<Entity> pooledEntities;

        float[] volume;

        protected override void OnCreate() {
            audioQuery = GetEntityQuery(new EntityQueryDesc {
                All = new[] {
                    ComponentType.ReadOnly<AudioClip>()
                }
            });

            pooledAudioQuery = GetEntityQuery(new EntityQueryDesc {
                All = new[] {
                    ComponentType.ReadOnly<FreeHandle>(),
                    ComponentType.ReadOnly<UsedHandle>()
                }
            });

            trackedEntities = new NativeList<Entity>(10, Allocator.Persistent);
            pooledEntities = new NativeList<Entity>(10, Allocator.Persistent);

            oneShotAudioSystem = World.GetOrCreateSystemManaged<OneShotAudioSystem>();
            commandBufferSystem = World
                .GetOrCreateSystemManaged<BeginPresentationEntityCommandBufferSystem>();
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

            if (pooledEntities.IsCreated) {
                pooledEntities.Dispose();
            }
        }

        protected override void OnUpdate() {
            new QueryAudioClipJob {
                AudioClipType = GetComponentTypeHandle<AudioClip>(true),
                EntityType = GetEntityTypeHandle(),
                TrackedEntities = trackedEntities,
            }.Run(audioQuery);

            new QueryPooledAudioJob {
                EntityType = GetEntityTypeHandle(),
                TrackedEntities = pooledEntities
            }.Run(pooledAudioQuery);
        }

        unsafe void OnLayout(UImGui.UImGui obj) {
            var audioClips = SystemAPI.GetComponentLookup<AudioClip>(true);
            var loadPaths = SystemAPI.GetComponentLookup<Path>(true);
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
                            changed = true;
                        }
                        break;
                    case AudioState.Playing:
                        if (ImGui.Button("Pause")) {
                            audioClip.CurrentState = AudioState.Paused;
                            changed = true;
                        }
                        break;
                    case AudioState.Stopped:
                        if (ImGui.Button("Play")) {
                            audioClip.CurrentState = AudioState.Playing;
                            changed = true;
                        }
                        break;
                }

                ImGui.SameLine();
                if (ImGui.Button("Stop")) {
                    audioClip.CurrentState = AudioState.Stopped;
                    changed = true;
                }

                var volume = math.sqrt(audioClip.Parameters.Volume);
                changed |= ImGui.SliderFloat("Volume", ref volume, 0, 1f);
                audioClip.Parameters.Volume = volume * volume;

                if (changed) {
                    commandBuffer.SetComponent(entity, audioClip);
                }
            }

            var freeHandles = SystemAPI.GetBufferLookup<FreeHandle>(true);
            // var usedHandles = GetBufferLookup<UsedHandle>(true);
            var paths = SystemAPI.GetComponentLookup<Path>(true);

            var audioCommandBuffer = oneShotAudioSystem.CreateCommandBuffer();

            for (int i = 0; i < pooledEntities.Length; i++) {
                var entity = pooledEntities[i];

                var freeHandleBuffer = freeHandles[entity].AsNativeArray();
                var path = paths[entity];

                ref var pathArray = ref path.Value.Value.Path;

                StringBuilder.Clear();
                for (int j = 0; j < pathArray.Length; j++) {
                    StringBuilder.Append(pathArray[j]);
                }

                ImGui.LabelText("Path", StringBuilder.ToString());
                StringBuilder.Clear();

                if (volume == null) {
                    volume = new float[freeHandleBuffer.Length];
                } else if (freeHandleBuffer.Length > volume.Length) {
                    System.Array.Resize(ref volume, freeHandleBuffer.Length);
                }

                for (int j = freeHandleBuffer.Length - 1; j >= 0; j--) {
                    StringBuilder.Append("Play Sound: ").Append(freeHandleBuffer[j].Value);
                    if (ImGui.Button(StringBuilder.ToString())) {
                        var p = new string((char*)pathArray.GetUnsafePtr());
                        audioCommandBuffer.Request(new FixedString512Bytes(p), volume[j]);
                    }

                    ImGui.SameLine();

                    StringBuilder.Clear().Append("Volume ").Append(j);
                    ImGui.SliderFloat(StringBuilder.ToString(), ref volume[j], 0, 1);
                    StringBuilder.Clear();
                }
            }
            ImGui.End();

            trackedEntities.Clear();
            pooledEntities.Clear();
        }
    }
}
