using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MiniAudio.Interop {

    public static class DefaultLogInitialization {

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LogHandler(string message);

#if UNITY_EDITOR_WIN && MINIAUDIO_DEVELOP
        public delegate void LoggerInitializationHandler(IntPtr log, IntPtr warn, IntPtr error);
        static LoggerInitializationHandler InitHandler;

        static void InitializeLogger(IntPtr log, IntPtr warn, IntPtr error) {
            InitHandler?.Invoke(log, warn, error);
        }
#endif

        static LogHandler DebugLogHandler;
        static LogHandler DebugWarnHandler;
        static LogHandler DebugErrorHandler;

        static IntPtr logFunctionPtr;
        static IntPtr warnFunctionPtr;
        static IntPtr errorFunctionPtr;
        public static void InitializeLibrary() {
#if UNITY_EDITOR_WIN && MINIAUDIO_DEVELOP
            InitHandler = LibraryHandler.GetDelegate<LoggerInitializationHandler>(ConstantImports.MiniAudioHandle, "InitializeLogger");
#endif
            DebugLogHandler = Debug.Log;
            DebugWarnHandler = Debug.LogWarning;
            DebugErrorHandler = Debug.LogError;

            logFunctionPtr = Marshal.GetFunctionPointerForDelegate(DebugLogHandler);
            warnFunctionPtr = Marshal.GetFunctionPointerForDelegate(DebugWarnHandler);
            errorFunctionPtr = Marshal.GetFunctionPointerForDelegate(DebugErrorHandler);
            InitializeLogger(logFunctionPtr, warnFunctionPtr, errorFunctionPtr);
        }

#if UNITY_EDITOR_WIN && MINIAUDIO_DEVELOP
#else
        [DllImport("MiniAudio_Unity_Bindings.dll")]
        static extern void InitializeLogger(IntPtr log, IntPtr warn, IntPtr error);
#endif
    }
}
