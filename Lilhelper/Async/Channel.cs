using System;
using System.Collections;
using System.Collections.Generic;

namespace Lilhelper.Async {
    /// <summary>
    /// 簡易型別安全通道：支援寫入、嘗試讀取、等待讀取（可設定逾時），並可作為列舉器消費。
    /// Simple type-safe channel supporting Write, TryRead, timed waiting Read, and enumeration.
    /// </summary>
    /// <typeparam name="T">通道元素型別。Type of element.</typeparam>
    public class Channel<T> : IEnumerable<T>, IWriteChannel<T>, IReadChannel<T>, IDisposable {
        private readonly IList<T> values = new List<T>();
        private readonly bool     isSelfCtx;
        private          bool     isWritten;

        public Ctx Ctx { get; }

        public Channel(Ctx ctx) {
            if (ctx is null) {
                Ctx  = new Ctx();
                isSelfCtx = true;
            } else {
                ctx.ThrowIfCancel();
                Ctx  = ctx;
                isSelfCtx = false;
            }
        }

        public void Write(T val) {
            Ctx?.ThrowIfCancel();
            values.Add(val);
            isWritten = true;
        }

        public IEnumerator Read(Action<T> onVal, double timeout = 15) {
            Ctx?.ThrowIfCancel();

            // 非依賴遊戲系統來處理時間
            // No dependency game system to handle time
            using var watcher = new OverTimeAlert(TimeSpan.FromSeconds(timeout));
            while (values.Count <= 0) {
                Ctx?.ThrowIfCancel();
                yield return watcher.YieldWatching;
            }

            var val = values[^1];
            values.RemoveAt(values.Count - 1);
            onVal?.Invoke(val);
        }

        public bool TryRead(Action<T> onVal) {
            Ctx?.ThrowIfCancel();
            if (!isWritten) return false;
            if (values.Count <= 0) return false;

            var val = values[^1];
            values.RemoveAt(values.Count - 1);
            onVal?.Invoke(val);
            return true;
        }

        public bool TryRead(out T result) {
            Ctx?.ThrowIfCancel();
            result = default;
            if (!isWritten) return false;
            if (values.Count <= 0) return false;
            result = values[^1];
            values.RemoveAt(values.Count - 1);
            return true;
        }

        public void Clear() {
            values.Clear();
            isWritten = false;
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

            values.Clear();
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
