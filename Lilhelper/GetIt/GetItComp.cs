using System;
using UnityEngine;
using Lilhelper.Objs;
using System.Collections.Generic;

namespace Lilhelper.GetIt {
    public enum ItLifeTime {
        /// 會一直活著不論何時 
        Always,
        /// 換場景時會清除參考
        Scene,
        /// 每一次 get 都會產生一個
        Instance,
    }

    public class GetItComp : MonoBehaviour {
        public static Lazy<GetItComp> I = new(() => {
            new GameObject($"[{nameof(GetItComp)}]")
                .AddCompOut(out GetItComp comp)
                .Keep();
            return comp;
        });

        private class FactoryKvp {
            public RuntimeTypeHandle typeHandle;
            public Delegate factory;
            public ItLifeTime lifeTime;
            public object cachedInstance;
        }

        private List<FactoryKvp> factories = new List<FactoryKvp>();

        private void Awake() {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnDestroy() {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private void OnSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1) {
            foreach (var kvp in factories) {
                if (kvp.lifeTime == ItLifeTime.Scene) {
                    kvp.cachedInstance = null;
                }
            }
        }

        public void Register<T>(Func<T> factory, ItLifeTime lifeTime = ItLifeTime.Scene) {
            factories.Add(new FactoryKvp {
                typeHandle = typeof(T).TypeHandle,
                factory = factory,
                lifeTime = lifeTime,
            });
        }

        public T Get<T>() {
            var targetType = typeof(T);
            foreach (var kvp in factories) {
                var registeredType = Type.GetTypeFromHandle(kvp.typeHandle);
                if (targetType.IsAssignableFrom(registeredType)) {
                    if (kvp.lifeTime == ItLifeTime.Instance) {
                        return (T)kvp.factory.DynamicInvoke();
                    }

                    if (!kvp.cachedInstance.DoExists()) {
                        kvp.cachedInstance = kvp.factory.DynamicInvoke();
                    }

                    return (T)kvp.cachedInstance;
                }
            }
            return default;
        }
    }
}