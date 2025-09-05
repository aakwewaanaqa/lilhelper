using System;
using Ct = System.Threading.CancellationToken;
using Cts = System.Threading.CancellationTokenSource;

namespace Lilhelper.Async {
    /// <summary>
    /// 一次性非同步事件取消事件用內容
    /// </summary>
    public class Ctx : IDisposable {
        private readonly Cts cts = new();

        /// <summary>
        /// 拷貝一個偵測原本的取消事件的標旗 <see cref="Ct"/>，
        /// 意思是說你的函式如果需要取消之後再執行一次，
        /// 就用來偵測取消，否則用 <see cref="Ctx"/> 就好
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public Ct Ct {
            get {
                cts.Token.ThrowIfCancellationRequested();
                return cts.Token;
            }
        }

        /// <summary>
        /// 如果取消就擲回例外
        /// </summary>
        /// <param name="onThrow">擲回例外之前的動作</param>
        public void ThrowIfCancel(Action onThrow = null) {
            if (!cts.IsCancellationRequested) return;
            onThrow?.Invoke();
            cts.Token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// 取消上一個，再建立新的一個
        /// </summary>
        /// <param name="ctx">用來儲存新的的欄位</param>
        /// <param name="ct">要跟著一起取消的標旗，如果 <see cref="ct"/> 被取消了，新的也會被取消</param>
        public static void CancelAnewLink(ref Ctx ctx, Ct ct) {
            ctx?.Cancel();
            ctx = new Ctx();
            ct.Register(ctx.Cancel);
        }

        /// <summary>
        /// 取消上一個，再建立新的一個
        /// </summary>
        /// <param name="ctx">用來儲存新的的欄位</param>
        public static void CancelAnew(ref Ctx ctx) {
            ctx?.Cancel();
            ctx?.Dispose();
            ctx = new Ctx();
        }

        /// <summary>
        /// 取消自己
        /// </summary>
        public void Cancel() {
            cts?.Cancel();
            cts?.Dispose();
        }

        /// <summary>
        /// 取消自己如果 <see cref="ctx"/> 取消的話
        /// </summary>
        /// <param name="ctx">另一個取消內容</param>
        public Ctx Register(Ctx ctx) {
            ctx.Ct.Register(Cancel);
            return this;
        }

        public static Ctx NewLinkAnother(Ctx ctx) {
            if (ctx is null) return new Ctx();
            return new Ctx().Register(ctx);
        }

        /// <summary>
        /// 當自己被取消時要觸發的事件
        /// </summary>
        /// <param name="onCancel">要觸發的事件</param>
        public void RegisterOnCancel(Action onCancel) {
            Ct.Register(onCancel);
        }

        public void Dispose() {
            cts?.Dispose();
        }
    }
}
