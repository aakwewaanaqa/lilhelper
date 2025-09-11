using UnityEngine;

namespace Lilhelper.Objs {
    public static class FloatExts {
        public static bool Approx(this float self, float other, float epsilon = 1e-6f) {
            return Mathf.Abs(self - other) < epsilon;
        }
    }
}
