using Lilhelper.Attributes;
using UnityEditor;
using UnityEngine;

namespace Lilhelper.Editors {
    [CustomPropertyDrawer(typeof(ReadOnly))]
    public class ReadOnlyDrawer : PropertyDrawer {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
            using var _ = new EditorGUI.DisabledScope(true);
            EditorGUI.PropertyField(rect, property);
        }
    }
}
