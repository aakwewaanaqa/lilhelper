using System;
using UnityEngine;

namespace Lilhelper.Debugs {
    [ExecuteInEditMode]
    public class RectTransformHelper : MonoBehaviour {
        [SerializeField] private RectTransform rectTransform;

        public Rect Rect {
            get => rectTransform.rect;
        }

        public Vector3 Pos {
            get => rectTransform.position;
            set => rectTransform.position = value;
        }

        private void Reset() {
            rectTransform = GetComponent<RectTransform>();
        }
    }
}
