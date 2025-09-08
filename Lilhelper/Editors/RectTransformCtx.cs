using Lilhelper.Objs;
using UnityEditor;
using UnityEngine;
using MenuCommand = UnityEditor.MenuCommand;

namespace Lilhelper.ContextMenu {
    public static class RectTransformCtx {
        [MenuItem("CONTEXT/RectTransform/Anchor Position", true)]
        public static bool AnchorPositionValid(MenuCommand command) {
            if (command.IsNull()) return false;
            if (command.context is not RectTransform r) return false;
            return r.parent.GetComponent<RectTransform>().IsExists();
        }

        [MenuItem("CONTEXT/RectTransform/Anchor Position")]
        public static void AnchorPosition(MenuCommand command) {
            var self   = command.context as RectTransform;
            var parent = self.parent.GetComponent<RectTransform>();

            var minMax = ((Vector2)self.position - ((Vector2)parent.position - parent.rect.position)) /
                parent.rect.size;
            self.anchorMin        = self.anchorMax = minMax;
            self.anchoredPosition = Vector2.zero;
            return;

            Vector2 GetPosition(RectTransform r) {
                return (Vector2)r.position - r.rect.position;
            }
        }
    }
}
