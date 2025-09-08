using System;
using Ct = System.Threading.CancellationToken;
using Cts = System.Threading.CancellationTokenSource;

namespace Lilhelper.Async {
    /// <summary>
    /// 一次性非同步事件取消事件用內容。
    /// One-shot async cancellation context wrapper.
    /// </summary>
    public class Ctx : IDisposable {
        private readonly Cts cts = new();

        /// <summary>
        /// 拷貝一個偵測原本的取消事件的標旗 <see cref="Ct"/>，
        /// 意思是說你的函式如果需要取消之後再執行一次，
        /// 就用來偵測取消，否則用 <see cref="Ctx"/> 就好。
        /// Copy a snapshot <see cref="Ct"/> of the underlying cancellation token.
        /// Use this token inside operations that may be cancelled and restarted; otherwise use <see cref="Ctx"/> APIs.
        /// </summary>
        /// <exception cref="NullReferenceException">當 <see cref="Ctx"/> 已取消時擲出。Thrown when this context is already cancelled.</exception>
        public Ct Ct {
            get {
                cts.Token.ThrowIfCancellationRequested();
                return cts.Token;
            }
        }

        /// <summary>
        /// 如果取消就擲回例外。
        /// Throw an exception if cancellation has been requested.
        /// </summary>
        /// <param name="onThrow">擲回例外之前的動作。Action to invoke before throwing.</param>
        public void ThrowIfCancel(Action onThrow = null) {
            if (!cts.IsCancellationRequested) return;
            onThrow?.Invoke();
            cts.Token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// 取消上一個，再建立新的一個。
        /// Cancel the previous context and create a new one linked to an external token.
        /// </summary>
        /// <param name="ctx">用來儲存新的的欄位。Field to store the new context.</param>
        /// <param name="ct">要跟著一起取消的標旗，如果該 token 被取消，新的也會被取消。External token to link; when it cancels, the new context cancels too.</param>
        public static void CancelAnewLink(ref Ctx ctx, Ct ct) {
            ctx?.Cancel();
            ctx = new Ctx();
            ct.Register(ctx.Cancel);
        }

        /// <summary>
        /// 取消上一個，再建立新的一個。
        /// Cancel the previous context and create a new one.
        /// </summary>
        /// <param name="ctx">用來儲存新的的欄位。Field to store the new context.</param>
        public static void CancelAnew(ref Ctx ctx) {
            ctx?.Cancel();
            ctx?.Dispose();
            ctx = new Ctx();
        }

        /// <summary>
        /// 取消自己。
        /// Cancel this context.
        /// </summary>
        public void Cancel() {
            cts?.Cancel();
            cts?.Dispose();
        }

        /// <summary>
        /// 取消自己如果 <see cref="ctx"/> 取消的話。
        /// Link this context to another; cancels when the other cancels.
        /// </summary>
        /// <param name="ctx">另一個取消內容。Another context to link.</param>
        public Ctx Register(Ctx ctx) {
            ctx.Ct.Register(Cancel);
            return this;
        }

        /// <summary>
        /// 建立一個新的 <see cref="Ctx"/> 並與既有的 <paramref name="ctx"/> 連動取消。
        /// Create a new <see cref="Ctx"/> and link its cancellation to the provided one.
        /// </summary>
        /// <param name="ctx">若非空，新的 Context 會在其被取消時一併取消。If not null, the new context will cancel when this one cancels.</param>
        public static Ctx NewLinkAnother(Ctx ctx) {
            if (ctx is null) return new Ctx();
            return new Ctx().Register(ctx);
        }

        /// <summary>
        /// 當自己被取消時要觸發的事件。
        /// Register a callback to be invoked when this context is cancelled.
        /// </summary>
        /// <param name="onCancel">要觸發的事件。Callback to invoke on cancellation.</param>
        public void RegisterOnCancel(Action onCancel) {
            Ct.Register(onCancel);
        }

        /// <summary>
        /// 釋放底層資源。
        /// Dispose underlying resources.
        /// </summary>
        public void Dispose() {
            cts?.Dispose();
        }
    }
}
