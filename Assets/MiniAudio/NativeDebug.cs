using System.Runtime.InteropServices;

namespace MiniAudio.Logging {

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LogHandler(string message);

    public static class NativeDebug {

        public static void Log(string message) {
            UnityEngine.Debug.Log(message);
        }

        public static void Warn(string message) {
            UnityEngine.Debug.LogWarning(message);
        }

        public static void Error(string message) {
            UnityEngine.Debug.LogError(message);
        }
    }
}
