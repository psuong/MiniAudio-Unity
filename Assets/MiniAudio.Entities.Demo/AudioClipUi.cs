using ImGuiNET;
using System.Collections.Generic;
using UImGui;
using UnityEngine;
using UnityEngine.UIElements;

namespace MiniAudio.Entities.Demo {

    public class AudioClipUi : MonoBehaviour {

        void OnEnable() {
            UImGuiUtility.Layout += OnLayout;
        }

        void OnDisable() {
            UImGuiUtility.Layout -= OnLayout;
        }

        void OnLayout(UImGui.UImGui obj) {
        }
    }
}
