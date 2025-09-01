using System;
using Lilhelper.Debugs;
using UnityEditor;

namespace Lilhelper.Editors {
    [CustomEditor(typeof(RectTransformHelper))]
    public class RectTransformHelperEditor : Editor {
        private RectTransformHelper comp;

        private void OnEnable() {
            comp = (RectTransformHelper)target;
        }

        public override void OnInspectorGUI() {
            comp.Pos = EditorGUILayout.Vector3Field("Pos", comp.Pos);
            EditorGUILayout.RectField("Rect", comp.Rect);
        }
    }
}
