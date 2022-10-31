using System;
using System.Text;
using ImGuiNET;
using MiniAudio.Entities.Systems;
using UImGui;
using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiniAudio.Entities.Demo {

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class AudioDrawingSystem : SystemBase {

        static readonly StringBuilder StringBuilder = new StringBuilder(256);

        float[] volume;
        float primaryVolume = 1.0f;
        bool initialized;

        protected override void OnStartRunning() {
            if (!initialized) {
                UImGuiUtility.Layout += OnLayout;
                initialized = true;
            }
        }

        protected override void OnDestroy() {
            UImGuiUtility.Layout -= OnLayout;
        }

        protected override void OnUpdate() { }

        unsafe void OnLayout(UImGui.UImGui obj) {
            if (ImGui.Button("Load SubScenes")) {
                var subScene = Object.FindObjectOfType<SubScene>();
                if (subScene != null & !subScene.IsLoaded) {
                    var sceneEntity = SceneSystem.GetSceneEntity(World.Unmanaged, subScene.SceneGUID);
                    if (!SceneSystem.IsSceneLoaded(World.Unmanaged, sceneEntity)) {
                        SceneSystem.LoadSceneAsync(World.Unmanaged, sceneEntity);
                    }
                }
            }
            StringBuilder.Clear().Append("File: ").Append(Application.streamingAssetsPath);

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var commandBuffer = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

            foreach (var (audioClip, path, entity) in 
                SystemAPI.Query<AudioClip, Path>().WithEntityAccess()) {

                ref var filePath = ref path.Value.Value.Path;
                for (int i = 0; i < filePath.Length; i++) {
                    StringBuilder.Append(filePath[i]);
                }

                ImGui.Text(StringBuilder.ToString());

                var audioClipCopy = audioClip;

                switch (audioClipCopy.CurrentState) {
                    case AudioState.Paused:
                        if (ImGui.Button("Resume")) {
                            audioClipCopy.CurrentState = AudioState.Playing;
                        }
                        break;
                    case AudioState.Playing:
                        if (ImGui.Button("Pause")) {
                            audioClipCopy.CurrentState = AudioState.Paused;
                        }

                        if (ImGui.SliderFloat("Volume", ref primaryVolume, 0f, 1.0f)) {
                            audioClipCopy.Parameters.Volume = primaryVolume;
                        }
                        break;
                    case AudioState.Stopped:
                        if (ImGui.Button("Play")) {
                            audioClipCopy.CurrentState = AudioState.Playing;
                        }
                        break;
                }

                ImGui.SameLine();
                if (ImGui.Button("Stop")) {
                    audioClipCopy.CurrentState = AudioState.Stopped;
                }

                if (audioClipCopy != audioClip) {
                    commandBuffer.SetComponent(entity, audioClipCopy);
                }
            }
            
            var oneShotAudioSystemSingleton = GetSingleton<OneShotAudioSystem.Singleton>();
            var audioCommandBuffer = oneShotAudioSystemSingleton.CreateCommandBuffer();

            ImGui.Begin("Pooled Audio");
            foreach (var (freeHandle, loadPath) in 
                SystemAPI
                    .Query<DynamicBuffer<FreeHandle>, Path>()
                    .WithAll<AudioPoolID>()) {

                ref var path = ref loadPath.Value.Value.Path;
                StringBuilder.Clear()
                    .Append("File: ")
                    .Append(new ReadOnlySpan<char>((char*)path.GetUnsafePtr(), path.Length));
                
                ImGui.Text(StringBuilder.ToString());

                if (volume == null) {
                    volume = new float[freeHandle.Length];
                } else if (freeHandle.Length > volume.Length) {
                    Array.Resize(ref volume, freeHandle.Length);
                }

                var play = StringBuilder.Clear().Append("Play").ToString();

                for (int i = freeHandle.Length - 1; i >= 0; i--) {
                    if (ImGui.Button(play)) {
                        audioCommandBuffer.Request(loadPath.Value.Value.ID, volume[i]);
                    }
                    
                    ImGui.SameLine();
                    StringBuilder.Clear().Append("Volume ").Append(i);
                    ImGui.SliderFloat(StringBuilder.ToString(), ref volume[i], 0, 1);
                }
                StringBuilder.Clear();
            }
            ImGui.End();
        }
    }
}
