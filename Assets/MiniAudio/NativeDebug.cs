using System.Runtime.InteropServices;

namespace MiniAudio.Unity.Logging {

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DebugLogHandler(string message);

    public static class NativeDebug {

        public static void Log(string message) {
            UnityEngine.Debug.Log(message);
        }
    }
}