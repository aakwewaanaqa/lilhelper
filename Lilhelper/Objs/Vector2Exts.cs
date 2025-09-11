using UnityEngine;

namespace Lilhelper.Objs {
    public static class Vector2Exts {
        public static bool Approx(this Vector2 self, Vector2 other, float epsilon = Consts.EPSILON) {
            return self.x.Approx(other.x, epsilon) && self.y.Approx(other.y, epsilon);
        }
    }
}
