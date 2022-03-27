using MiniAudio.Interop;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MiniAudio.Entities.Systems {

    public partial class PlaySoundSystem : SystemBase {

#if !UNITY_EDITOR
        [BurstCompile]
#endif
        struct PlayAudioJob : IJobEntityBatch {

            public ComponentTypeHandle<FixedAudioStateHistory> FixedAudioStateType;

            public ComponentTypeHandle<AudioClip> AudioClipType;

            public EntityCommandBuffer CommandBuffer;

            public void Execute(ArchetypeChunk batchInChunk, int batchIndex) {
                var audioClips = batchInChunk.GetNativeArray(AudioClipType);
                var stateTypes = batchInChunk.GetNativeArray(FixedAudioStateType);

                for (int i = 0; i < batchInChunk.Count; i++) {
                    var audioClip = audioClips[i];
                    var stateType = stateTypes[i];

                    if (audioClip.CurrentState == AudioState.Playing && stateType.Value == AudioState.Stopped && audioClip.Handle != uint.MaxValue) {
                        stateType.Value = audioClip.CurrentState;
                        MiniAudioHandler.PlaySound(audioClip.Handle);
                    }

                    stateTypes[i] = stateType;
                }
            }
        }
        protected override void OnUpdate() {
        }
    }
}
