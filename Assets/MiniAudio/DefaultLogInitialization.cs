using MiniAudio.Interop;
using MiniAudio.Logging;
using System;
using System.Runtime.InteropServices;

namespace MiniAudio {

    public static class DefaultLogInitialization {

#if UNITY_EDITOR
        public delegate void LoggerInitializationHandler(IntPtr log, IntPtr warn, IntPtr error);
        static LoggerInitializationHandler InitHandler;

        static void InitializeLogger(IntPtr log, IntPtr warn, IntPtr error) {
            InitHandler?.Invoke(log, warn, error);
        }
#else
        [DllImport("MiniAudio_Unity_Bindings")]
        static extern void InitializeLogger(IntPtr log, IntPtr warn, IntPtr error);

#endif

        static LogHandler DebugLogHandler;
        static LogHandler DebugWarnHandler;
        static LogHandler DebugErrorHandler;

        static IntPtr logFunctionPtr;
        static IntPtr warnFunctionPtr;
        static IntPtr errorFunctionPtr;

        public static void InitializeLibrary() {
            InitHandler = LibraryHandler.GetDelegate<LoggerInitializationHandler>(CommonImports.LibraryHandle, "InitializeLogger");
            DebugLogHandler = new LogHandler(NativeDebug.Log);
            DebugWarnHandler = new LogHandler(NativeDebug.Warn);
            DebugErrorHandler = new LogHandler(NativeDebug.Error);

            logFunctionPtr = Marshal.GetFunctionPointerForDelegate(DebugLogHandler);
            warnFunctionPtr = Marshal.GetFunctionPointerForDelegate(DebugWarnHandler);
            errorFunctionPtr = Marshal.GetFunctionPointerForDelegate(DebugErrorHandler);
            InitializeLogger(logFunctionPtr, warnFunctionPtr, errorFunctionPtr);
        }
    }
}
