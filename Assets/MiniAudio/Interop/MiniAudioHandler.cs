using System.Runtime.InteropServices;

namespace MiniAudio.Interop {

#if UNITY_EDITOR
    public delegate bool MiniEngineInitializationCheckHandler();
    public delegate void MiniAudioEngineHandler();
    public delegate uint MiniAudioLoadHandler(string path, SoundLoadParameters loadParams);
    public delegate void MiniSoundHandler(uint handle);
#endif

    public static class MiniAudioHandler {
#if UNITY_EDITOR
        static MiniEngineInitializationCheckHandler InitializationCheckHandler;
        static MiniAudioEngineHandler InitializationHandler;
        static MiniAudioLoadHandler LoadSoundHandler;
        static MiniSoundHandler PlaySoundHandler;
        static MiniSoundHandler StopSoundHandler;
        static MiniAudioEngineHandler ReleaseHandler;

        public static void InitializeLibrary() {
            var library = ConstantImports.MiniAudioHandle;
            InitializationCheckHandler = LibraryHandler.GetDelegate<MiniEngineInitializationCheckHandler>(library, "IsEngineInitialized");
            // TODO: Fix the InitializedEngine function naming
            InitializationHandler = LibraryHandler.GetDelegate<MiniAudioEngineHandler>(library, "InitializedEngine");
            LoadSoundHandler = LibraryHandler.GetDelegate<MiniAudioLoadHandler>(library, "LoadSound");
            PlaySoundHandler = LibraryHandler.GetDelegate<MiniSoundHandler>(library, "PlaySound");
            StopSoundHandler = LibraryHandler.GetDelegate<MiniSoundHandler>(library, "StopSound");
            ReleaseHandler = LibraryHandler.GetDelegate<MiniAudioEngineHandler>(library, "ReleaseEngine");
        }

        public static bool IsEngineInitialized() {
            if (InitializationCheckHandler != null) {
                return InitializationCheckHandler.Invoke();
            }
            return false;
        }

        public static void InitializeEngine() {
            InitializationHandler?.Invoke();
        }

        public static uint LoadSound(string path, SoundLoadParameters loadParams) {
            return LoadSoundHandler.Invoke(path, loadParams);
        }

        public static void PlaySound(uint handle) {
            PlaySoundHandler?.Invoke(handle);
        }

        public static void StopSound(uint handle) {
            StopSoundHandler?.Invoke(handle);
        }

        public static void ReleaseEngine() {
            ReleaseHandler?.Invoke();
        }
#endif
    }
}
