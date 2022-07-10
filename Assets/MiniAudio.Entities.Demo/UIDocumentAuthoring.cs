using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace MiniAudio.Entities.Demo {

    [RequireComponent(typeof(UIDocument))]

    public class AudioClipUi : MonoBehaviour {

        internal static AudioClipUi Instance { get; private set; }

        internal readonly List<TemplateContainer> Containers = new List<TemplateContainer>();

        public VisualTreeAsset ClipAsset;

        UIDocument document;

        void Awake() {
            if (Instance != null) {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(this);
            document = GetComponent<UIDocument>();
            Instance = this;

            TemplateContainer template = ClipAsset.CloneTree();
            document.rootVisualElement.Add(template);
        }

        void ApplyVolume(ChangeEvent<float> changeEvent) {

        }
    }
}
