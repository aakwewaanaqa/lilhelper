using System;

using Lilhelper.Objs;

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

}

public static class StateExts {
    public static State<T> ToState<T>(this T value) {
        return new State<T>(value);
    }

    public static void ActListen<T>(this State<T> state, Action<T> listener) {
        if (state.IsNull()) return;
        listener(state.Value);
        state.AddListener(listener);
    }
}