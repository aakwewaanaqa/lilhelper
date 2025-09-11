using UnityEngine;

namespace Lilhelper.Objs {
    public static class FloatExts {
        public static bool Approx(this float self, float other, float epsilon = Consts.EPSILON) {
            return Mathf.Abs(self - other) < epsilon;
        }
    }
}
