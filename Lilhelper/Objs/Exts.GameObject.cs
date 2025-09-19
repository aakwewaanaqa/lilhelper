using System;
using System.Collections;

using UnityEngine;

using Object = UnityEngine.Object;

namespace Lilhelper.Objs {
    public static partial class Exts {
        public static GameObject Instantiate(this GameObject self, Transform parent = null) {
            if (parent.IsNull()) return Object.Instantiate(self);
            return Object.Instantiate(self, parent);
        }

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
            if (apply.DoExists()) o.StartCoroutine(apply(o));
            return self;
        }

        /// <summary>
        /// Get an existing component of type T on the GameObject, or throw if missing.
        /// 取得並輸出指定類型的組件；若不存在則拋出錯誤。
        /// </summary>
        /// <typeparam name="T">要取得的組件型別。Type of component to get.</typeparam>
        /// <param name="self">目標 GameObject。Target GameObject.</param>
        /// <param name="o">輸出取得的組件。Output component.</param>
        /// <returns>回傳原本的 GameObject，方便鏈式呼叫。Returns the same GameObject for chaining.</returns>
        /// <exception cref="NullReferenceException">當組件不存在時由 RaiseErr.NullRef 拋出。Thrown via RaiseErr.NullRef when component is missing.</exception>
        public static GameObject GetCompOut<T>(this GameObject self, out T o) {
            o = self.GetComponent<T>();
            if (o.IsNull()) RaiseErr.NullRef($"Component {typeof(T).Name} not found on GameObject {self.name}");
            return self;
        }

        /// <summary>
        /// Get an existing component and apply an action to it; throw if the component is missing.
        /// 取得指定組件並對其執行動作；若不存在則拋出錯誤。
        /// </summary>
        /// <typeparam name="T">要取得的組件型別。Type of component to get.</typeparam>
        /// <param name="self">目標 GameObject。Target GameObject.</param>
        /// <param name="apply">要套用於組件的動作。Action to apply to the component.</param>
        /// <returns>回傳原本的 GameObject，方便鏈式呼叫。Returns the same GameObject for chaining.</returns>
        /// <exception cref="NullReferenceException">當組件不存在時由 RaiseErr.NullRef 拋出。Thrown via RaiseErr.NullRef when component is missing.</exception>
        public static GameObject GetCompAct<T>(this GameObject self, Action<T> apply) {
            var o = self.GetComponent<T>();
            if (o.IsNull()) RaiseErr.NullRef($"Component {typeof(T).Name} not found on GameObject {self.name}");
            apply?.Invoke(o);
            return self;
        }

        /// <summary>
        /// Get an existing component, apply an action, and output it; throw if the component is missing.
        /// 取得指定組件、套用動作並輸出；若不存在則拋出錯誤。
        /// </summary>
        /// <typeparam name="T">要取得的組件型別。Type of component to get.</typeparam>
        /// <param name="self">目標 GameObject。Target GameObject.</param>
        /// <param name="apply">要套用於組件的動作。Action to apply to the component.</param>
        /// <param name="o">輸出取得的組件。Output component.</param>
        /// <returns>回傳原本的 GameObject，方便鏈式呼叫。Returns the same GameObject for chaining.</returns>
        /// <exception cref="NullReferenceException">當組件不存在時由 RaiseErr.NullRef 拋出。Thrown via RaiseErr.NullRef when component is missing.</exception>
        public static GameObject GetCompActOut<T>(this GameObject self, Action<T> apply, out T o) {
            o = self.GetComponent<T>();
            if (o.IsNull()) RaiseErr.NullRef($"Component {typeof(T).Name} not found on GameObject {self.name}");
            apply?.Invoke(o);
            return self;
        }

        /// <summary>
        /// Get an existing component and, if a coroutine factory is provided, start the coroutine on it.
        /// 取得指定組件，若提供協程工廠則在該組件上啟動協程。
        /// 若組件不存在則拋出錯誤。
        /// </summary>
        /// <typeparam name="T">要取得的組件型別。Type of component to get.</typeparam>
        /// <param name="self">目標 GameObject。Target GameObject.</param>
        /// <param name="apply">協程工廠；可為 null。Coroutine factory to start; can be null.</param>
        /// <returns>回傳原本的 GameObject，方便鏈式呼叫。Returns the same GameObject for chaining.</returns>
        /// <exception cref="NullReferenceException">當組件不存在時由 RaiseErr.NullRef 拋出。Thrown via RaiseErr.NullRef when component is missing.</exception>
        public static GameObject GetCompCo<T>(this GameObject self, Func<T, IEnumerator> apply) {
            var o = self.GetComponent<T>();
            if (o.IsNull()) RaiseErr.NullRef($"Component {typeof(T).Name} not found on GameObject {self.name}");
            if (apply.DoExists() && o is MonoBehaviour mono) mono.StartCoroutine(apply(o));
            return self;
        }

        /// <summary>
        /// Get an existing component, optionally start a coroutine on it, and output the component.
        /// 取得指定組件，可選擇在其上啟動協程，並輸出該組件。
        /// 若組件不存在則拋出錯誤。
        /// </summary>
        /// <typeparam name="T">要取得的組件型別。Type of component to get.</typeparam>
        /// <param name="self">目標 GameObject。Target GameObject.</param>
        /// <param name="apply">協程工廠；可為 null。Coroutine factory to start; can be null.</param>
        /// <param name="o">輸出取得的組件。Output component.</param>
        /// <returns>回傳原本的 GameObject，方便鏈式呼叫。Returns the same GameObject for chaining.</returns>
        /// <exception cref="NullReferenceException">當組件不存在時由 RaiseErr.NullRef 拋出。Thrown via RaiseErr.NullRef when component is missing.</exception>
        public static GameObject GetCompCoOut<T>(this GameObject self, Func<T, IEnumerator> apply, out T o) {
            o = self.GetComponent<T>();
            if (o.IsNull()) RaiseErr.NullRef($"Component {typeof(T).Name} not found on GameObject {self.name}");
            if (apply.DoExists() && o is MonoBehaviour mono) mono.StartCoroutine(apply(o));
            return self;
        }

        /// <summary>
        /// Add a component of type T to the GameObject and apply an action to it.
        /// 新增指定型別的組件至 GameObject，並對其套用動作。
        /// </summary>
        /// <typeparam name="T">要新增的組件型別。Type of component to add.</typeparam>
        /// <param name="self">目標 GameObject。Target GameObject.</param>
        /// <param name="apply">新增後要套用於組件的動作；可為 null。Action to apply to the newly added component; can be null.</param>
        /// <returns>回傳原本的 GameObject，方便鏈式呼叫。Returns the same GameObject for chaining.</returns>
        public static GameObject AddCompAct<T>(this GameObject self, Action<T> apply) where T : Component {
            var o = self.AddComponent<T>();
            apply?.Invoke(o);
            return self;
        }
    }
}
