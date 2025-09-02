using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Lilhelper.Objs {
    public static class Exts {
        public static bool IsNull(this object self) {
            return self is null || self.Equals(null);
        }

        public static bool IsExists(this object self) {
            return self is not null && !self.Equals(null);
        }

        public static GameObject EnsureComp<T>(this GameObject self) where T : Component {
            var comp = self.GetComponent<T>();
            if (comp.IsNull()) self.AddComponent<T>();
            return self;
        }

        public static GameObject EnsureComp<T>(this GameObject self, Action<T> apply) where T : Component {
            var comp                = self.GetComponent<T>();
            if (comp.IsNull()) comp = self.AddComponent<T>();
            apply?.Invoke(comp);
            return self;
        }

        public static GameObject EnsureComp<T>(this GameObject self, Func<T, IEnumerator> apply) where T : MonoBehaviour {
            var comp                = self.GetComponent<T>();
            if (comp.IsNull()) comp = self.AddComponent<T>();
            if (apply.IsExists()) comp.StartCoroutine(apply(comp));
            return self;
        }


        public static Transform EnsureComp<T>(this Transform self) where T : Component {
            var comp                = self.GetComponent<T>();
            if (comp.IsNull()) comp = self.gameObject.AddComponent<T>();
            return comp.IsNull() ? self.gameObject.AddComponent<T>().transform : self;
        }

        public static Transform EnsureComp<T>(this Transform self, Action<T> apply) where T : Component {
            var comp                = self.GetComponent<T>();
            if (comp.IsNull()) comp = self.gameObject.AddComponent<T>();
            apply?.Invoke(comp);
            return self;
        }

        public static Component EnsureComp<T>(this Component self) where T : Component {
            var comp = self.GetComponent<T>();
            if (comp.IsNull()) self.gameObject.AddComponent<T>();
            return self;
        }

        public static Component EnsureComp<T>(this Component self, Action<T> apply) where T : Component {
            var comp                = self.GetComponent<T>();
            if (comp.IsNull()) comp = self.gameObject.AddComponent<T>();
            apply?.Invoke(comp);
            return self;
        }

        public static GameObject Instantiate(this GameObject self) {
            return GameObject.Instantiate(self);
        }
    }
}
