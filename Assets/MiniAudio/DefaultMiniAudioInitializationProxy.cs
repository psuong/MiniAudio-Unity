using MiniAudio.Interop;
using MiniAudio.Logging;
using UnityEngine;

namespace MiniAudio {

    public class DefaultMiniAudioInitializationProxy : MonoBehaviour {

        public string Path;
        public string Path2;

        int handle;

        void OnEnable() {
            ConstantImports.Initialize();
            DefaultLogInitialization.InitializeLibrary();
            MiniAudioHandler.InitializeLibrary();
        }

        void Start() {
            MiniAudioHandler.InitializeEngine();
            // handle = MiniAudioHandler.LoadSound(Path, new SoundLoadParameters {
            //     Volume = 1.0f
            // });
        }

        void Update() {
            // if (Input.GetKeyUp(KeyCode.Space)) {
            //     MiniAudioHandler.StopSound(handle);
            // }
        }

        void OnDisable() {
            MiniAudioHandler.ReleaseEngine();
            ConstantImports.Release();
        }
    }
}
