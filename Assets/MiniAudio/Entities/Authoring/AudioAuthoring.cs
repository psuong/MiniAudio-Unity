using MiniAudio.Interop;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace MiniAudio.Entities.Authoring {

    public class AudioAuthoring : MonoBehaviour, IConvertGameObjectToEntity {

        public bool IsPathStreamingAssets;
        public string Path;
        public SoundLoadParameters Parameters;

        public unsafe void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            if (string.IsNullOrEmpty(Path)) {
                return;
            }

            var buffer = dstManager.AddBuffer<LoadPath>(entity);
            buffer.ResizeUninitialized(Path.Length);
            fixed (char* head = Path) {
                UnsafeUtility.MemCpy(buffer.GetUnsafePtr(), head, sizeof(char) * Path.Length);
            }

            var audioClip = AudioClip.New();
            audioClip.Parameters = Parameters;
            dstManager.AddComponentData(entity, audioClip);
            dstManager.AddComponentData(entity, new FixedAudioStateHistory {
                Value = audioClip.CurrentState
            });

            dstManager.AddComponentData(entity, new StreamingPathMetadata {
                IsStreamingAssetPath = IsPathStreamingAssets
            });
        }
    }
}
