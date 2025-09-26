#if UNITY_UGUI
using Lilhelper.Objs;
using Lilhelper.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Lilhelper.Editors {
    [CustomEditor(typeof(RectTransformHelper))]
    public class RectTransformHelperEditor : Editor {
        private RectTransformHelper comp;
        private Image               image;

        private bool IsPreserveAspect {
            get {
                if (image.IsNull()) return comp.PreserveAspect;
                return image.preserveAspect;
            }
            set {
                if (image.IsNull()) comp.PreserveAspect = value;
                else image.preserveAspect = value;
            }
        }

        private void OnEnable() {
            comp  = (RectTransformHelper)target;
            image = comp.GetComponent<Image>();
        }

        public override void OnInspectorGUI() {
            {
                using var disabled = new EditorGUI.DisabledGroupScope(true);
                using var box      = new EditorGUILayout.VerticalScope(EditorStyles.helpBox);
                EditorGUILayout.ObjectField(image, typeof(Image), true);
            }
            {
                using var disabled = new EditorGUI.DisabledGroupScope(true);
                using var box      = new EditorGUILayout.VerticalScope(EditorStyles.helpBox);
                comp.Pos = EditorGUILayout.Vector3Field(nameof(comp.Pos), comp.Pos);
                EditorGUILayout.RectField("Rect", comp.Rect);
            }
            {
                using var scope = new EditorGUILayout.VerticalScope(EditorStyles.helpBox);
                IsPreserveAspect = EditorGUILayout.Toggle(nameof(IsPreserveAspect), IsPreserveAspect);
                if (IsPreserveAspect) {
                    float x     = comp.SizeDelta.x;
                    float y     = comp.SizeDelta.y;
                    float ratio = x / y;
                    comp.SizeDelta = EditorGUILayout.Vector2Field(nameof(comp.SizeDelta), comp.SizeDelta);
                    float newX  = comp.SizeDelta.x;
                    float newY  = comp.SizeDelta.y;
                    if (!Mathf.Approximately(newX,      x)) newY = newX / ratio;
                    else if (!Mathf.Approximately(newY, y)) newX = newY * ratio;
                    comp.SizeDelta = new Vector2(newX, newY);
                } else {
                    comp.SizeDelta = EditorGUILayout.Vector2Field(nameof(comp.SizeDelta), comp.SizeDelta);
                }
            }
        }
    }
}
#endif
