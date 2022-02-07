using UnityEngine;

namespace MiniAudio {

    public class DefaultMiniAudioInitializationProxy : MonoBehaviour {

        public string Path;
        public string Path2;

        void OnEnable() {
            CommonImports.Initialize();
            DefaultLogInitialization.InitializeLibrary();
            MiniAudioHandler.InitializeLibrary();
        }

        void Start() {
            MiniAudioHandler.InitializeEngine();
            MiniAudioHandler.PlaySound(Path);
            MiniAudioHandler.PlaySound(Path2);
        }


        void OnDisable() {
            MiniAudioHandler.ReleaseEngine();
        }
    }
}