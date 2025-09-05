using System;
using System.Collections;
using System.Collections.Generic;

namespace Lilhelper.Async {
    public class Channel<T> : IEnumerable<T>, IWriteChannel<T>, IReadChannel<T>, IDisposable {
        private readonly IList<T> values = new List<T>();
        private readonly Ctx      ctx;
        private readonly bool     isSelfCtx;

        public Ctx Ctx => ctx;

        public Channel(Ctx ctx) {
            if (ctx is null) {
                this.ctx = new Ctx();
                isSelfCtx = true;
            } else {
                ctx.ThrowIfCancel();
                this.ctx = ctx;
                isSelfCtx = false;
            }
        }

        public void Write(T val) {
            ctx?.ThrowIfCancel();
            values.Add(val);
        }

        public IEnumerator Read(Action<T> onVal) {
            ctx?.ThrowIfCancel();

            while (values.Count <= 0) {
                ctx?.ThrowIfCancel();
                yield return null;
            }

            var val = values[^1];
            values.RemoveAt(values.Count - 1);
            onVal?.Invoke(val);
        }

        public bool TryRead(out T result) {
            ctx?.ThrowIfCancel();
            var val = values[^1];
            values.RemoveAt(values.Count - 1);
            return val;
        }

        public void Dispose() {
            if (isSelfCtx) {
                ctx?.Cancel();
                ctx?.Dispose();
            }

            foreach (var val in values) {
                if (val is IDisposable disposable) {
                    disposable.Dispose();
                }
            }
        }

        public IEnumerator<T> GetEnumerator() {
            ctx?.ThrowIfCancel();

            while (values.Count > 0) {
                ctx?.ThrowIfCancel();
                yield return values[0];
                values.RemoveAt(0);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
