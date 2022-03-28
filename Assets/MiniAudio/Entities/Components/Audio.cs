using System.Runtime.InteropServices;
using MiniAudio.Interop;
using Unity.Entities;

namespace MiniAudio.Entities {

    [System.Flags]
    public enum AudioState : byte {
        Stopped = 1 << 0,
        Playing = 1 << 1,
        Paused = 1 << 2
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct LoadPath : IBufferElementData {
        public char Value;
    }

    public struct StreamingPathMetadata : IComponentData {
        public bool IsStreamingAssetPath;
    }

    public struct AudioClip : IComponentData {

        /// <summary>
        /// Stores the index in which MiniAudio allocated the sound.
        /// </summary>
        public uint Handle;

        /// <summary>
        /// Stores the current AudioState.
        /// </summary>
        public AudioState CurrentState;

        /// <summary>
        /// Stores the SoundLoadParameters needed to initially load the sound.
        /// </summary>
        public SoundLoadParameters Parameters;

        public static AudioClip New() {
            return new AudioClip {
                Handle = uint.MaxValue,
                CurrentState = AudioState.Stopped,
                Parameters = new SoundLoadParameters {
                    Volume = 1.0f,
                }
            };
        }
    }

    /// <summary>
    /// Stores the last known state of the AudioClip.
    /// </summary>
    internal struct AudioStateHistory : IComponentData {
        public AudioState Value;
    }
}
