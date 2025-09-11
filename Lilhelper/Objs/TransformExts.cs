using UnityEngine;

namespace Lilhelper.Objs {
    public static class TransformExts {

        public static Transform SetPos(this Transform self, Vector3 pos) {
            self.position = pos;

            return self;
        }

        public static Transform SetScale(this Transform self, Vector3 scale) {
            self.localScale = scale;

            return self;
        }

    }
}
