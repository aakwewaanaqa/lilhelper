using System;

using UnityEngine;

namespace Lilhelper.Reactive {
    public class OnDestroyComp : MonoBehaviour, IDisposable {
        public Action onDestroy;

        public void OnDestroy() {
            onDestroy?.Invoke();
            onDestroy = null;
        }

        public void Dispose() {
            Destroy(this);
        }
    }
}