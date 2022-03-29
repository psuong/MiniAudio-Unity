namespace MiniAudio.Interop {

    public static unsafe class MiniAudioHandler {
#if UNITY_EDITOR
        public delegate bool MiniEngineInitializationCheckHandler();
        public delegate void MiniAudioEngineHandler();
        public delegate uint MiniAudioLoadHandler(string path, SoundLoadParameters loadParams);
        public delegate uint UnsafeMiniAudioLoadHandler(char* path, uint sizeInBytes, SoundLoadParameters loadParams);
        public delegate void MiniSoundHandler(uint handle);
        public delegate void MiniStopSoundHandler(uint handle, bool rewind);
        public delegate bool MiniSoundStateHandler(uint handle);
        public delegate void MiniSoundVolumeHandler(uint handle, float volume);
#endif

#if UNITY_EDITOR
        static MiniEngineInitializationCheckHandler InitializationCheckHandler;
        static MiniAudioEngineHandler InitializationHandler;
        static MiniAudioLoadHandler LoadSoundHandler;
        static UnsafeMiniAudioLoadHandler UnsafeLoadSoundHandler;
        static MiniSoundHandler PlaySoundHandler;
        static MiniStopSoundHandler StopSoundHandler;
        static MiniAudioEngineHandler ReleaseHandler;
        static MiniSoundStateHandler SoundPlayingHandler;
        static MiniSoundVolumeHandler SoundVolumeHandler;

        public static void InitializeLibrary() {
            var library = ConstantImports.MiniAudioHandle;
            InitializationCheckHandler = LibraryHandler
                .GetDelegate<MiniEngineInitializationCheckHandler>(library, "IsEngineInitialized");
            InitializationHandler = LibraryHandler
                .GetDelegate<MiniAudioEngineHandler>(library, "InitializeEngine");
            LoadSoundHandler = LibraryHandler.GetDelegate<MiniAudioLoadHandler>(library, "LoadSound");
            UnsafeLoadSoundHandler = LibraryHandler.GetDelegate<UnsafeMiniAudioLoadHandler>(library, "UnsafeLoadSound");
            PlaySoundHandler = LibraryHandler.GetDelegate<MiniSoundHandler>(library, "PlaySound");
            StopSoundHandler = LibraryHandler.GetDelegate<MiniStopSoundHandler>(library, "StopSound");
            ReleaseHandler = LibraryHandler.GetDelegate<MiniAudioEngineHandler>(library, "ReleaseEngine");
            SoundPlayingHandler = LibraryHandler.GetDelegate<MiniSoundStateHandler>(library, "IsSoundPlaying");
            SoundVolumeHandler = LibraryHandler.GetDelegate<MiniSoundVolumeHandler>(library, "SetSoundVolume");
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

        public static uint UnsafeLoadSound(char* path, uint sizeInBytes, SoundLoadParameters loadParams) {
            if (UnsafeLoadSoundHandler == null) {
                return uint.MaxValue;
            }
            return UnsafeLoadSoundHandler.Invoke(path, sizeInBytes, loadParams);
        }

        public static void PlaySound(uint handle) {
            PlaySoundHandler?.Invoke(handle);
        }

        public static void StopSound(uint handle, bool rewind) {
            StopSoundHandler?.Invoke(handle, rewind);
        }

        public static void SetSoundVolume(uint handle, float volume) {
            SoundVolumeHandler?.Invoke(handle, volume);
        }

        public static bool IsSoundPlaying(uint handle) {
            if (SoundPlayingHandler == null) {
                return false;
            }
            return SoundPlayingHandler.Invoke(handle);
        }

        public static void ReleaseEngine() {
            ReleaseHandler?.Invoke();
        }
#endif
    }
}
