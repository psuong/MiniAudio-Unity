using System.Text;
using InitialPrefabs.NimGui;
using MiniAudio.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MiniAudio.Entities.Demo {
    
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class AudioDrawingSystem : SystemBase {

        static readonly StringBuilder StringBuilder = new StringBuilder(256);

        [BurstCompile]
        partial struct AudioQueryJob : IJobEntity {

            [WriteOnly] 
            public NativeList<AudioClip> Clips;

            [WriteOnly]
            public NativeList<Entity> AssociatedEntities;

            void Execute(Entity entity, in AudioClip audioClip) {
                Clips.AddNoResize(audioClip);
                AssociatedEntities.AddNoResize(entity);
            }
        }

        EntityQuery audioQuery;
        EntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate() {
            audioQuery = GetEntityQuery(new EntityQueryDesc {
                All = new [] {
                    ComponentType.ReadOnly<AudioClip>() 
                }
            });

            commandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            var audioHandles = new NativeList<AudioClip>(
                audioQuery.CalculateEntityCount(), 
                Allocator.TempJob);

            var entities=  new NativeList<Entity>(
                audioQuery.CalculateEntityCount(), 
                Allocator.TempJob);

            new AudioQueryJob() {
                Clips = audioHandles,
                AssociatedEntities = entities
            }.Run(audioQuery);

            var center = new float2(Screen.width / 2f, Screen.height / 2f);

            using (var pane = new ImPane("Audio Handles", center, 500)) {
                if (pane.IsVisible) {
                    for (int i = 0; i < audioHandles.Length; i++) {
                        var audioHandle = audioHandles[i];
                        StringBuilder.Clear().Append("Audio Handle: ").Append(audioHandle.Handle);
                        ImGui.Label(StringBuilder);

                        switch (audioHandle.CurrentState) {
                            case AudioState.Stopped:
                                if (ImGui.Button("Play")) {
                                    audioHandle.CurrentState = AudioState.Playing;
                                    var commandBuffer = commandBufferSystem.CreateCommandBuffer();
                                    commandBuffer.SetComponent(entities[i], audioHandle);
                                }
                                break;
                            case AudioState.Playing:
                                if (ImGui.Button("Stop")) {
                                    audioHandle.CurrentState = AudioState.Stopped;
                                    var commandBuffer = commandBufferSystem.CreateCommandBuffer();
                                    commandBuffer.SetComponent(entities[i], audioHandle);
                                }
                                break;
                            case AudioState.Paused:
                                break;
                        }
                    }
                }
            }

            commandBufferSystem.AddJobHandleForProducer(Dependency);

            audioHandles.Dispose();
            entities.Dispose();
        }
    }
}
