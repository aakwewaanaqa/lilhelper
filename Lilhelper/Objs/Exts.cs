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

        public static GameObject EnsureComp<T>(this GameObject self, out T o) where T : Component {
            o = self.GetComponent<T>();
            if (o.IsNull()) o = self.AddComponent<T>();
            return self;
        }

        public static GameObject EnsureCompAct<T>(this GameObject self, Action<T> apply, out T o) where T : Component {
            o = self.GetComponent<T>();
            if (o.IsNull()) o = self.AddComponent<T>();
            apply?.Invoke(o);
            return self;
        }

        public static GameObject EnsureCompCo<T>(this GameObject self, Func<T, IEnumerator> apply, out T o)
            where T : MonoBehaviour {
            o = self.GetComponent<T>();
            if (o.IsNull()) o = self.AddComponent<T>();
            if (apply.IsExists()) o.StartCoroutine(apply(o));
            return self;
        }


        public static Transform EnsureComp<T>(this Transform self, out T o) where T : Component {
            o = self.GetComponent<T>();
            if (o.IsNull()) o = self.gameObject.AddComponent<T>();
            return o.IsNull() ? self.gameObject.AddComponent<T>().transform : self;
        }

        public static Transform EnsureComp<T>(this Transform self, Action<T> apply, out T o) where T : Component {
            o = self.GetComponent<T>();
            if (o.IsNull()) o = self.gameObject.AddComponent<T>();
            apply?.Invoke(o);
            return self;
        }

        public static Component EnsureComp<T>(this Component self, out T o) where T : Component {
            o = self.GetComponent<T>();
            if (o.IsNull()) o = self.gameObject.AddComponent<T>();
            return self;
        }

        public static Component EnsureComp<T>(this Component self, Action<T> apply, out T o) where T : Component {
            o = self.GetComponent<T>();
            if (o.IsNull()) o = self.gameObject.AddComponent<T>();
            apply?.Invoke(o);
            return self;
        }

        public static GameObject Instantiate(this GameObject self, Transform parent = null) {
            if (parent.IsNull()) return GameObject.Instantiate(self);
            return GameObject.Instantiate(self, parent);
        }

        public static T Out<T>(this T self, out T t) {
            t = self;
            return self;
        }
    }
}
