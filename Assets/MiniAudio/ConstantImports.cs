using MiniAudio.Interop;
using System;
using UnityEngine;

namespace MiniAudio {

    public static class ConstantImports {
#if UNITY_EDITOR
        public const string MiniAudioLibPath = "/MiniAudio/Plugins/MiniAudio_Unity_Bindings.dll";
        public static IntPtr MiniAudioHandle => LibraryHandleInternal;

        static IntPtr LibraryHandleInternal;

        public static void Initialize() {
            LibraryHandleInternal = LibraryHandler.InitializeLibrary(Application.dataPath + MiniAudioLibPath);
        }
#endif
    }
}
