namespace MiniAudio.Interop {

    public delegate void MiniAudioEngineHandler();
    public delegate void PlaySoundHandler(string path);

    public static class MiniAudioHandler {

        static MiniAudioEngineHandler InitializationHandler;
        static MiniAudioEngineHandler ReleaseHandler;
        static PlaySoundHandler PlaySoundHandler;

        public static void InitializeLibrary() {
            throw new System.NotImplementedException();
        }

        public static void InitializeEngine() {
            throw new System.NotImplementedException();
        }

        public static void ReleaseEngine() {
            throw new System.NotImplementedException();
        }

        public static void PlaySound(string path) {
            throw new System.NotImplementedException();
        }
    }
}
