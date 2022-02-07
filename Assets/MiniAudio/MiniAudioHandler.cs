using MiniAudio.Interop;

namespace MiniAudio {

    public delegate void MiniAudioEngineHandler();
    public delegate void PlaySoundHandler(string path);

    public static class MiniAudioHandler {

        static MiniAudioEngineHandler InitializationHandler;
        static MiniAudioEngineHandler ReleaseHandler;
        static PlaySoundHandler PlaySoundHandler;

        public static void InitializeLibrary() {
            InitializationHandler = LibraryHandler.GetDelegate<MiniAudioEngineHandler>(CommonImports.LibraryHandle, "initialize_engine_handle");
            ReleaseHandler = LibraryHandler.GetDelegate<MiniAudioEngineHandler>(CommonImports.LibraryHandle, "release_engine");
            PlaySoundHandler = LibraryHandler.GetDelegate<PlaySoundHandler>(CommonImports.LibraryHandle, "play_sound");
        }

        public static void InitializeEngine() {
            InitializationHandler?.Invoke();
        }

        public static void ReleaseEngine() {
            ReleaseHandler?.Invoke();
        }

        public static void PlaySound(string path) {
            PlaySoundHandler?.Invoke(path);
        }
    }
}
