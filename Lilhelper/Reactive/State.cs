using System;

using Lilhelper.Objs;

using UnityEngine;

namespace Lilhelper.Reactive {

    public class State<T> : IDisposable {
        private T value;

        public State(T initialValue = default) {
            value = initialValue;
        }

        public T Value {
            get => value;
            set {
                if (!Equals(this.value, value)) {
                    this.value = value;
                    OnChanged?.Invoke(this.value);
                }
            }
        }

        public void AddListener(Action<T> listener) {
            OnChanged += listener;
        }

        public void Dispose() {
            OnChanged = null;
        }

        public event Action<T> OnChanged;

        public static implicit operator T(State<T> state) {
            return state.Value;
        }

        public static implicit operator State<T>(T value) {
            return new State<T>(value);
        }
    }

    public static class StateExts {
        public static State<T> ToState<T>(this T value) {
            return new State<T>(value);
        }

        public static void ActListenOfHost<T>(this State<T> state, Action<T> listener, GameObject host) {
            if (state.IsNull()) return;
            if (listener.IsNull()) return;
            if (host.IsNull()) throw new ArgumentNullException(nameof(host));
            listener(state.Value);
            state.AddListener(listener);
            host.EnsureCompAct<OnDestroyComp>(it => {
                it.onDestroy += () => state.OnChanged -= listener;
            });
        }
    }
}