using UnityEngine;

namespace Lilhelper.UI {
    [ExecuteAlways]
    [AddComponentMenu("Lilhelper/UI/Rect Transform Helper")]
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
