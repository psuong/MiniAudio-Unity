namespace MiniAudio.Interop {

    public unsafe static class MiniAudioHandler {
#if UNITY_EDITOR
        public delegate bool MiniEngineInitializationCheckHandler();
        public delegate void MiniAudioEngineHandler();
        public delegate uint MiniAudioLoadHandler(string path, SoundLoadParameters loadParams);
        public delegate uint UnsafeMiniAudioLoadHandler(char* path, uint length, SoundLoadParameters loadParams);
        public delegate void MiniSoundHandler(uint handle);
#endif

#if UNITY_EDITOR
        static MiniEngineInitializationCheckHandler InitializationCheckHandler;
        static MiniAudioEngineHandler InitializationHandler;
        static MiniAudioLoadHandler LoadSoundHandler;
        static UnsafeMiniAudioLoadHandler UnsafeLoadSoundHandler;
        static MiniSoundHandler PlaySoundHandler;
        static MiniSoundHandler StopSoundHandler;
        static MiniAudioEngineHandler ReleaseHandler;

        public static void InitializeLibrary() {
            var library = ConstantImports.MiniAudioHandle;
            InitializationCheckHandler = LibraryHandler
                .GetDelegate<MiniEngineInitializationCheckHandler>(library, "IsEngineInitialized");
            InitializationHandler = LibraryHandler
                .GetDelegate<MiniAudioEngineHandler>(library, "InitializeEngine");
            LoadSoundHandler = LibraryHandler.GetDelegate<MiniAudioLoadHandler>(library, "LoadSound");
            UnsafeLoadSoundHandler = LibraryHandler.GetDelegate<UnsafeMiniAudioLoadHandler>(library, "UnsafeLoadSound");
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
            if (LoadSoundHandler == null) {
                return uint.MaxValue;
            }
            return LoadSoundHandler.Invoke(path, loadParams);
        }

        public static uint UnsafeLoadSound(char* path, uint length, SoundLoadParameters loadParams) {
            if (UnsafeLoadSoundHandler == null) {
                return uint.MaxValue;
            }
            return UnsafeLoadSoundHandler.Invoke(path, length, loadParams);
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
