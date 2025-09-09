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

                /// <summary>
                /// 通道中目前元素數量。
                /// Current number of elements stored in the channel.
                /// </summary>
                public int Length => values.Count;
        
        /// <summary>
        /// 取消內容，用於跨 API 傳遞/檢查取消狀態。
        /// Cancellation context used to propagate/check cancellation across APIs.
        /// </summary>
        public Ctx Ctx { get; }

        /// <summary>
        /// 以指定的取消內容建立通道；若為 null 則自建並自行管理生命週期。
        /// Create a channel with the given cancellation context; if null, create and own one.
        /// </summary>
        /// <param name="ctx">取消內容；null 代表自建。Cancellation context; null means self-owned context will be created.</param>
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

        /// <summary>
        /// 寫入一筆資料到通道，若已取消則拋出例外。
        /// Write a single value into the channel; throws if the context is cancelled.
        /// </summary>
        /// <param name="val">要寫入的值。The value to write.</param>
        public void Write(T val) {
            Ctx?.ThrowIfCancel();
            values.Add(val);
            isWritten = true;
        }

        /// <summary>
        /// 非同步讀取：若暫時無資料則以逾時監視進行等待，最後以回調取回值。
        /// Asynchronous read: waits (with timeout) until an element is available, then invokes the callback.
        /// </summary>
        /// <param name="onVal">取得值後的回調。Callback invoked with the retrieved value.</param>
        /// <param name="timeout">等待的逾時秒數。Timeout in seconds for waiting.</param>
        /// <returns>可在 Unity 協程中使用的列舉器。Enumerator usable in Unity coroutines.</returns>
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

        /// <summary>
        /// 嘗試立即讀取一筆資料；不會等待，若無可用資料回傳 false。
        /// Try to read a value immediately; does not wait and returns false if none available.
        /// </summary>
        /// <param name="onVal">成功讀取時回調該值。Callback invoked with the value when read succeeds.</param>
        /// <returns>是否成功讀取。Whether a value was read.</returns>
        public bool TryRead(Action<T> onVal) {
            Ctx?.ThrowIfCancel();
            if (!isWritten) return false;
            if (values.Count <= 0) return false;

            var val = values[^1];
            values.RemoveAt(values.Count - 1);
            onVal?.Invoke(val);
            return true;
        }

        /// <summary>
        /// 嘗試立即讀取一筆資料（out 版本）；不會等待，若無可用資料回傳 false。
        /// Try to read a value immediately (out overload); does not wait and returns false if none available.
        /// </summary>
        /// <param name="result">成功讀取時輸出的值。The output value when read succeeds.</param>
        /// <returns>是否成功讀取。Whether a value was read.</returns>
        public bool TryRead(out T result) {
            Ctx?.ThrowIfCancel();
            result = default;
            if (!isWritten) return false;
            if (values.Count <= 0) return false;
            result = values[^1];
            values.RemoveAt(values.Count - 1);
            return true;
        }

        /// <summary>
        /// 清除所有暫存的元素並重置寫入狀態。
        /// Clear all buffered elements and reset write state.
        /// </summary>
        public void Clear() {
            values.Clear();
            isWritten = false;
        }

        /// <summary>
        /// 釋放通道持有的資源；若上下文由本實例建立，將一併取消並釋放之。
        /// Dispose resources held by the channel; if the context is self-owned, cancel and dispose it as well.
        /// </summary>
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

        /// <summary>
        /// 以先入先出順序列舉通道中的元素。
        /// Enumerate elements in FIFO order.
        /// </summary>
        /// <returns>元素的列舉器。Enumerator over elements.</returns>
        public IEnumerator<T> GetEnumerator() {
            Ctx?.ThrowIfCancel();

            while (values.Count > 0) {
                Ctx?.ThrowIfCancel();
                yield return values[0];
                values.RemoveAt(0);
            }
        }

        /// <summary>
        /// 取得非泛型列舉器的明確實作。
        /// Explicit non-generic enumerator implementation.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
