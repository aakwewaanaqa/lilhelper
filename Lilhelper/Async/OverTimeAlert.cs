using System;
using System.Collections;
using System.Diagnostics;

namespace Lilhelper.Async {
    /// <summary>
    /// 逾時監視器：在協程反覆 yield 檢查經過時間，超過指定門檻即擲出 TimeoutException。
    /// Timeout watcher: used in coroutines to check elapsed time on each yield and throw TimeoutException when exceeded.
    /// </summary>
    public class OverTimeAlert : IDisposable {
        private          Stopwatch watch;
        private readonly TimeSpan  timeSpan;

        /// <summary>
        /// 建立逾時監視器，指定容許經過時間。
        /// Create a timeout watcher with the allowed elapsed time threshold.
        /// </summary>
        /// <param name="timeSpan">逾時門檻 The timeout threshold.</param>
        public OverTimeAlert(TimeSpan timeSpan) {
            watch         = Stopwatch.StartNew();
            this.timeSpan = timeSpan;
        }

        /// <summary>
        /// 於協程中反覆 yield 使用，若已超過門檻將擲出 <see cref="TimeoutException"/>。
        /// Use this in coroutine loops; throws <see cref="TimeoutException"/> when elapsed exceeds threshold.
        /// </summary>
        public IEnumerator YieldWatching {
            get {
                if (watch.Elapsed > timeSpan) throw new TimeoutException();
                yield return null;
            }
        }

        /// <summary>
        /// 停止計時並釋放資源。
        /// Stop the stopwatch and release resources.
        /// </summary>
        public void Dispose() {
            watch.Stop();
            watch = null;
        }
    }
}
