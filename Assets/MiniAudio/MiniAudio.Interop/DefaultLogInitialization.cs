using MiniAudio.Interop;
using System;
using System.Runtime.InteropServices;

namespace MiniAudio.Interop {

    public static class DefaultLogInitialization {

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
            DebugLogHandler = NativeDebug.Log;
            DebugWarnHandler = NativeDebug.Warn;
            DebugErrorHandler = NativeDebug.Error;

            logFunctionPtr = Marshal.GetFunctionPointerForDelegate(DebugLogHandler);
            warnFunctionPtr = Marshal.GetFunctionPointerForDelegate(DebugWarnHandler);
            errorFunctionPtr = Marshal.GetFunctionPointerForDelegate(DebugErrorHandler);
            InitializeLogger(logFunctionPtr, warnFunctionPtr, errorFunctionPtr);
        }
    }
}
