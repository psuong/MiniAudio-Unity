using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MiniAudio.Unity.Logging {
    public static class MiniAudioInitialization {
#if UNITY_EDITOR
        internal static IntPtr LibraryHandle;

        public static DebugLogHandler LogHandler;
#endif
#if UNITY_EDITOR_WIN
        const string LIB_PATH = "/MiniAudio/Plugins/MiniAudio.Bindings.Unity.dll";

        [DllImport("kernel32")]
        static extern IntPtr LoadLibrary(string path);

        [DllImport("kernel32")]
        static extern IntPtr GetProcAddress(IntPtr libraryHandle, string symbolName);

        [DllImport("kernel32")]
        static extern bool FreeLibrary(IntPtr libraryHandle);

        public static IntPtr InitializeLibrary(string path) {
            IntPtr handle = LoadLibrary(path);

            if (handle == IntPtr.Zero) {
                throw new Exception("Couldn't open native library: " + path);
            }
            return handle;
        }

        public static void ReleaseLibrary(IntPtr libraryPtr) {
            Debug.Log("Closing external library");
            FreeLibrary(libraryPtr);
        }

        public static T GetDelegate<T>(IntPtr libraryPtr, string functionName) where T : class {
            IntPtr symbol = GetProcAddress(libraryPtr, functionName);

            if (symbol == IntPtr.Zero) {
                throw new Exception($"Could not find function: {functionName}");
            }

            return Marshal.GetDelegateForFunctionPointer(symbol, typeof(T)) as T;
        }
#endif
    }
}