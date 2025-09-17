using System;
using System.Collections;

using UnityEngine;

namespace Lilhelper.Objs {
    public static partial class Exts {
        /// <summary>
        /// Ensures that the specified component of type T exists on a GameObject.
        /// If the component does not exist, it will be added.
        /// 確保在 GameObject 上存在指定类型的组件。
        /// 如果组件不存在，则将其添加。
        /// </summary>
        /// <typeparam name="T">
        /// The type of the component to ensure.
        /// 組件的類型。
        /// </typeparam>
        /// <param name="self">
        /// The GameObject to check for the component.
        /// 目標 GameObject。
        /// </param>
        /// <param name="o">
        /// The output variable to store the retrieved or added component of type T.
        /// 方便存取已取得或已添加的组件。
        /// </param>
        /// <returns>
        /// The GameObject that contains the ensured component.
        /// 還是回傳原本的 GameObject，方便連續呼叫。
        /// </returns>
        public static GameObject EnsureComp<T>(this GameObject self, out T o) where T : Component {
            o = self.GetComponent<T>();
            if (o.IsNull()) o = self.AddComponent<T>();
            return self;
        }

        /// <summary>
        /// 確保組件存在並套用動作。
        /// Ensure component exists on GameObject and apply an action.
        /// </summary>
        /// <param name="apply">對組件進行設定的動作。Action to apply on the component.</param>
        /// <param name="o">輸出取得/新增的組件。Output ensured component.</param>
        public static GameObject EnsureCompAct<T>(this GameObject self, Action<T> apply, out T o) where T : Component {
            o = self.GetComponent<T>();
            if (o.IsNull()) o = self.AddComponent<T>();
            apply?.Invoke(o);
            return self;
        }

        /// <summary>
        /// 確保組件存在並在 MonoBehaviour 上啟動協程。
        /// Ensure component exists and start a coroutine on the MonoBehaviour.
        /// </summary>
        /// <param name="apply">要啟動的協程工廠。Coroutine factory to start.</param>
        /// <param name="o">輸出取得/新增的組件。Output ensured component.</param>
        public static GameObject EnsureCompCo<T>(this GameObject self, Func<T, IEnumerator> apply, out T o)
            where T : MonoBehaviour {
            o = self.GetComponent<T>();
            if (o.IsNull()) o = self.AddComponent<T>();
            if (apply.IsExists()) o.StartCoroutine(apply(o));
            return self;
        }

        public static GameObject GetCompOut<T>(this GameObject self, out T o) {
            o = self.GetComponent<T>();
            if (o.IsNull()) RaiseErr.NullRef($"Component {typeof(T).Name} not found on GameObject {self.name}");
            return self;
        }
        
        public static GameObject GetCompAct<T>(this GameObject self, Action<T> apply) {
            var o = self.GetComponent<T>();
            if (o.IsNull()) RaiseErr.NullRef($"Component {typeof(T).Name} not found on GameObject {self.name}");
            apply?.Invoke(o);
            return self;
        }

        public static GameObject GetCompActOut<T>(this GameObject self, Action<T> apply, out T o) {
            o = self.GetComponent<T>();
            if (o.IsNull()) RaiseErr.NullRef($"Component {typeof(T).Name} not found on GameObject {self.name}");
            apply?.Invoke(o);
            return self;
        }

        public static GameObject GetCompCo<T>(this GameObject self, Func<T, IEnumerator> apply) {
            var o = self.GetComponent<T>();
            if (o.IsNull()) RaiseErr.NullRef($"Component {typeof(T).Name} not found on GameObject {self.name}");
            if (apply.IsExists() && o is MonoBehaviour mono) mono.StartCoroutine(apply(o));
            return self;
        }

        public static GameObject GetCompCoOut<T>(this GameObject self, Func<T, IEnumerator> apply, out T o) {
            o = self.GetComponent<T>();
            if (o.IsNull()) RaiseErr.NullRef($"Component {typeof(T).Name} not found on GameObject {self.name}");
            if (apply.IsExists() && o is MonoBehaviour mono) mono.StartCoroutine(apply(o));
            return self;
        }

        public static GameObject AddCompAct<T>(this GameObject self, Action<T> apply) where T : Component {
            var o = self.AddComponent<T>();
            apply?.Invoke(o);
            return self;
        }
    }
}
