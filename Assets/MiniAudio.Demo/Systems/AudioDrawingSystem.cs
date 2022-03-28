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

            void Execute(in AudioClip audioClip) {
                Clips.Add(audioClip);
            }
        }

        EntityQuery audioQuery;

        protected override void OnCreate() {
            audioQuery = GetEntityQuery(new EntityQueryDesc {
                All = new [] {
                    ComponentType.ReadOnly<AudioClip>() 
                }
            });
        }

        protected override void OnUpdate() {
            var audioHandles = new NativeList<AudioClip>(audioQuery.CalculateEntityCount(), Allocator.TempJob);

            new AudioQueryJob() {
                Clips = audioHandles
            }.Run(audioQuery);

            var center = new float2(Screen.width / 2f, Screen.height / 2f);

            using (var pane = new ImPane("Audio Handles", center, 500)) {
                if (pane.IsVisible) {
                    for (int i = 0; i < audioHandles.Length; i++) {
                        var audioHandle = audioHandles[i];
                        StringBuilder.Clear().Append("Audio Handle: ").Append(audioHandle.Handle);
                        ImGui.Label(StringBuilder);
                    }
                }
            }

            audioHandles.Dispose();
        }
    }
}
