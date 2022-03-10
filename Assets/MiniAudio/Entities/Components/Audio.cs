using MiniAudio.Interop;
using Unity.Entities;

namespace MiniAudio.Entities {

    public enum AudioState : byte {
        Stopped,
        Playing,
        Paused
    }

    public struct LoadPath : IBufferElementData {
        public char Value;
    }

    public struct AudioClip : IComponentData {
        public uint Handle;
        public AudioState State;
        public SoundLoadParameters Parameters;

        public static AudioClip New() {
            return new AudioClip {
                Handle = uint.MaxValue,
                State = AudioState.Stopped,
                Parameters = new SoundLoadParameters {
                    Volume = 1.0f
                }
            };
        }
    }
}
