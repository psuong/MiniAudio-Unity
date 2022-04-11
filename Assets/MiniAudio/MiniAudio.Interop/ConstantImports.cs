﻿using System;
using UnityEngine;

namespace MiniAudio.Interop {

    public static class ConstantImports {

        public const string MiniAudioLibPath = "/MiniAudio/Plugins/MiniAudio_Unity_Bindings.dll";
        public static IntPtr MiniAudioHandle => LibraryHandleInternal;

        static IntPtr LibraryHandleInternal;

        public static void Initialize() {
            LibraryHandleInternal = LibraryHandler.InitializeLibrary(Application.dataPath + MiniAudioLibPath);
        }

        public static void Release() {
            LibraryHandler.ReleaseLibrary(MiniAudioHandle);
        }
    }
}