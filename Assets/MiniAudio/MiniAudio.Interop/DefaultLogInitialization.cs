using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MiniAudio.Interop {

    public static class DefaultLogInitialization {

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LogHandler(string message);

        public delegate void LoggerInitializationHandler(IntPtr log, IntPtr warn, IntPtr error);
        static LoggerInitializationHandler InitHandler;

        static void InitializeLogger(IntPtr log, IntPtr warn, IntPtr error) {
            InitHandler?.Invoke(log, warn, error);
        }

        static LogHandler DebugLogHandler;
        static LogHandler DebugWarnHandler;
        static LogHandler DebugErrorHandler;

        static IntPtr logFunctionPtr;
        static IntPtr warnFunctionPtr;
        static IntPtr errorFunctionPtr;
        public static void InitializeLibrary() {
            InitHandler = LibraryHandler.GetDelegate<LoggerInitializationHandler>(ConstantImports.MiniAudioHandle, "InitializeLogger");
            DebugLogHandler = Debug.Log;
            DebugWarnHandler = Debug.LogWarning;
            DebugErrorHandler = Debug.LogError;

            logFunctionPtr = Marshal.GetFunctionPointerForDelegate(DebugLogHandler);
            warnFunctionPtr = Marshal.GetFunctionPointerForDelegate(DebugWarnHandler);
            errorFunctionPtr = Marshal.GetFunctionPointerForDelegate(DebugErrorHandler);
            InitializeLogger(logFunctionPtr, warnFunctionPtr, errorFunctionPtr);
        }
    }
}
