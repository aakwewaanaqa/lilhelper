using Lilhelper.Flutterive.Widgets;

using UnityEngine;

namespace Lilhelper.Tests {
    [CreateAssetMenu(fileName = "SampleTestResources", menuName = "Lilhelper/Tests/SampleTestResources")]
    public class SampleTestResources : ScriptableObject {
        public Sprite sampleSprite;

        [ContextMenu("Sample Test")]
        public void SampleTest() {
            sampleSprite.ImageWidget(sizing: new Size(100, 50)).RootWidget().Build();
        }
    }
}