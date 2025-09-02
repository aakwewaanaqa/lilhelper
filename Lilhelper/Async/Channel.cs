using System;
using System.Collections;
using System.Collections.Generic;
using Ct = System.Threading.CancellationToken;

namespace Lilhelper.Async {
    public class Channel<T> : IDisposable, IEnumerable<T> {
        private readonly IList<T> values = new List<T>();
        public readonly  Ctx      ctx;

        public Channel(Ctx ctx) {
            this.ctx = ctx;
        }

        public Channel(Ct ct) {
            Ctx.CancelAnewLink(ref ctx, ct);
        }

        public Channel() {
            ctx = new Ctx();
        }

        public void Write(T val) {
            values.Add(val);
        }

        public T Read() {
            while (values.Count < 0) { }

            var val = values[^1];
            values.RemoveAt(values.Count - 1);
            return val;
        }

        public void Dispose() {
            ctx?.Cancel();
            ctx?.Dispose();
            foreach (var val in values) {
                if (val is IDisposable disposable) {
                    disposable.Dispose();
                }
            }
        }

        public IEnumerator<T> GetEnumerator() {
            while (values.Count < 0) { }

            while (values.Count > 0) {
                yield return values[0];
                values.RemoveAt(0);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
