using MiniAudio.Interop;
using UnityEngine;

namespace MiniAudio {

    public class DefaultMiniAudioInitializationProxy : MonoBehaviour {

        public string Path;
        public string Path2;

        uint handle;

        void OnEnable() {
            ConstantImports.Initialize();
            DefaultLogInitialization.InitializeLibrary();
#if UNITY_EDITOR_WIN && MINIAUDIO_DEVELOP
            MiniAudioHandler.InitializeLibrary();
#endif
        }

        void Start() {
            MiniAudioHandler.InitializeEngine();
        }

        void OnDisable() {
            MiniAudioHandler.ReleaseEngine();
            ConstantImports.Release();
        }
    }
}
