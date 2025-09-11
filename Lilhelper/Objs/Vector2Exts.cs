using UnityEngine;

namespace Lilhelper.Objs {
    public static class Vector2Exts {
        public static bool Approx(this Vector2 self, Vector2 other) {
            return self.x.Approx(other.x) && self.y.Approx(other.y);
        }
    }
}
