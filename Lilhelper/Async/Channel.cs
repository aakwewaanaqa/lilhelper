using System;
using System.Collections;
using System.Collections.Generic;

namespace Lilhelper.Async {
    public class Channel<T> : IEnumerable<T>, IWriteChannel<T>, IReadChannel<T>, IDisposable {
        private readonly IList<T> values = new List<T>();
        private readonly bool     isSelfCtx;

        public Ctx Ctx { get; }

        public Channel(Ctx ctx) {
            if (ctx is null) {
                this.Ctx  = new Ctx();
                isSelfCtx = true;
            } else {
                ctx.ThrowIfCancel();
                this.Ctx  = ctx;
                isSelfCtx = false;
            }
        }

        public void Write(T val) {
            Ctx?.ThrowIfCancel();
            values.Add(val);
        }

        public IEnumerator Read(Action<T> onVal) {
            Ctx?.ThrowIfCancel();

            while (values.Count <= 0) {
                Ctx?.ThrowIfCancel();
                yield return null;
            }

            var val = values[^1];
            values.RemoveAt(values.Count - 1);
            onVal?.Invoke(val);
        }

        public bool TryRead(Action<T> onVal) {
            Ctx?.ThrowIfCancel();
            if (values.Count <= 0) return false;

            var val = values[^1];
            values.RemoveAt(values.Count - 1);
            onVal?.Invoke(val);
            return true;
        }

        public void Dispose() {
            if (isSelfCtx) {
                Ctx?.Cancel();
                Ctx?.Dispose();
            }

            foreach (var val in values) {
                if (val is IDisposable disposable) {
                    disposable.Dispose();
                }
            }
        }

        public IEnumerator<T> GetEnumerator() {
            Ctx?.ThrowIfCancel();

            while (values.Count > 0) {
                Ctx?.ThrowIfCancel();
                yield return values[0];
                values.RemoveAt(0);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
