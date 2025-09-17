using System;
using System.Collections;
using UnityEngine;

namespace Lilhelper.Objs {
    /// <summary>
    /// 常用物件與 Unity 組件的輔助擴充方法。
    /// Common helper extension methods for objects and Unity components.
    /// </summary>
    public static partial class Exts {
        /// <summary>
        /// 判斷是否為 Unity 意義下的 null（包含被銷毀的 UnityEngine.Object）。
        /// Determine if the object is null in Unity sense (including destroyed UnityEngine.Object).
        /// </summary>
        public static bool IsNull(this object self) {
            return self is null || self.Equals(null);
        }

        /// <summary>
        /// 判斷物件是否存在（非 Unity 虛擬 null）。
        /// Determine if the object exists (not Unity's fake null).
        /// </summary>
        public static bool IsExists(this object self) {
            return self is not null && !self.Equals(null);
        }

        /// <summary>
        /// 在 Transform 所屬的 GameObject 上確保存在指定類型的組件。
        /// Ensure that a component of type T exists on the Transform's GameObject.
        /// </summary>
        /// <typeparam name="T">組件型別。Component type.</typeparam>
        /// <param name="self">目標 Transform。Target transform.</param>
        /// <param name="o">輸出取得/新增的組件。Output ensured component.</param>
        /// <returns>原始 Transform（或其所屬 GameObject 的 Transform）。The original Transform.</returns>
        public static Transform EnsureComp<T>(this Transform self, out T o) where T : Component {
            o = self.GetComponent<T>();
            if (o.IsNull()) o = self.gameObject.AddComponent<T>();
            return o.IsNull() ? self.gameObject.AddComponent<T>().transform : self;
        }

        /// <summary>
        /// 確保組件存在並套用動作。
        /// Ensure component exists on GameObject and apply an action.
        /// </summary>
        /// <param name="apply">對組件進行設定的動作。Action to apply on the component.</param>
        /// <param name="o">輸出取得/新增的組件。Output ensured component.</param>
        public static Transform EnsureCompAct<T>(this Transform self, Action<T> apply, out T o) where T : Component {
            o = self.GetComponent<T>();
            if (o.IsNull()) o = self.gameObject.AddComponent<T>();
            apply?.Invoke(o);
            return self;
        }


        /// <summary>
        /// 在 Transform 所屬的 GameObject 上確保存在指定類型的組件，並對其套用動作。
        /// Ensure a component of type T exists on the Transform's GameObject and apply an action to it.
        /// </summary>
        /// <typeparam name="T">組件型別。Component type.</typeparam>
        /// <param name="self">目標 Transform。Target transform.</param>
        /// <param name="apply">要套用於組件的動作。Action to apply on the component.</param>
        /// <param name="o">輸出取得/新增的組件。Output ensured component.</param>
        /// <returns>原始 Transform。The original Transform.</returns>
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
