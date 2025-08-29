using System;
using UnityEngine;

namespace Lilhelper.Objs {
    public static class Exts {
        public static bool IsNull(this object self) {
            return self is null && self.Equals(null);
        }

        public static bool IsExists(this object self) {
            return self is not null && !self.Equals(null);
        }

        public static GameObject EnsureComp<T>(this GameObject self) where T : Component {
            var comp = self.GetComponent<T>();
            return comp.IsNull() ? self.AddComponent<T>().gameObject : self;
        }

        public static GameObject EnsureComp<T>(this GameObject self, Action<T> apply) where T : Component {
            var comp = self.GetComponent<T>();
            apply?.Invoke(comp);
            return comp.IsNull() ? self.AddComponent<T>().gameObject : self;
        }

        public static Transform EnsureComp<T>(this Transform self) where T : Component {
            var comp = self.GetComponent<T>();
            return comp.IsNull() ? self.gameObject.AddComponent<T>().transform : self;
        }

        public static Transform EnsureComp<T>(this Transform self, Action<T> apply) where T : Component {
            var comp = self.GetComponent<T>();
            apply?.Invoke(comp);
            return comp.IsNull() ? self.gameObject.AddComponent<T>().transform : self;
        }

        public static Component EnsureComp<T>(this Component self) where T : Component {
            var comp = self.GetComponent<T>();
            return comp.IsNull() ? self.gameObject.AddComponent<T>() : comp;
        }

        public static Component EnsureComp<T>(this Component self, Action<T> apply) where T : Component {
            var comp = self.GetComponent<T>();
            apply?.Invoke(comp);
            return comp.IsNull() ? self.gameObject.AddComponent<T>() : comp;
        }

        public static GameObject Instantiate(this GameObject self) {
            return GameObject.Instantiate(self);
        }
    }
}
