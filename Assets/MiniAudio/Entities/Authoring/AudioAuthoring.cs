using MiniAudio.Interop;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace MiniAudio.Entities.Authoring {

    public class AudioAuthoring : MonoBehaviour, IConvertGameObjectToEntity {

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

            dstManager.AddComponentData(entity, AudioClip.New());
        }
    }
}
