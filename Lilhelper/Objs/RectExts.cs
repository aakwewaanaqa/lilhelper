using UnityEngine;

namespace Lilhelper.Objs {
    public static class RectExts {
        public static (Rect, Rect) SplitV(this Rect self, float height) {
            return (
                new Rect(self.x, self.y,          self.width, height),
                new Rect(self.x, self.y + height, self.width, self.height - height));
        }

        public static (Rect, Rect) SplitH(this Rect self, float width) {
            return (
                new Rect(self.x,         self.y, width,              self.height),
                new Rect(self.x + width, self.y, self.width - width, self.height));
        }
    }
}
