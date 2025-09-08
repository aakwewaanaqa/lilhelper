using UnityEngine;

namespace Lilhelper.UI {
    [ExecuteAlways]
    [AddComponentMenu("Lilhelper/UI/Rect Transform Helper")]
    /// <summary>
    /// RectTransform 輔助元件：提供常用屬性存取與維持寬高比選項。
    /// RectTransform helper: exposes common properties and an option to preserve aspect.
    /// </summary>
    public class RectTransformHelper : MonoBehaviour {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private bool          preserveAspect;

        public bool PreserveAspect {
            get => preserveAspect;
            set => preserveAspect = value;
        }

        public Rect Rect {
            get => rectTransform.rect;
        }

        public Vector3 Pos {
            get => rectTransform.position;
            set => rectTransform.position = value;
        }

        public Vector2 SizeDelta {
            get => rectTransform.sizeDelta;
            set => rectTransform.sizeDelta = value;
        }

        private void Reset() {
            rectTransform = GetComponent<RectTransform>();
        }
    }
}
