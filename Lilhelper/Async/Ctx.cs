using System;
using Ct  = System.Threading.CancellationToken;
using Cts = System.Threading.CancellationTokenSource;

namespace Lilhelper.Async {
    public class Ctx : IDisposable {

        private Cts cts;

        public Ctx() {
            cts = new Cts();
        }

        public Ct ct => cts?.Token ?? throw new NullReferenceException();

        public void ThrowIfCancel(Action onThrow = null) {
            onThrow?.Invoke();
            ct.ThrowIfCancellationRequested();
        }

        public static void CancelAnewLink(ref Ctx ctx, Ct ct) {
            ctx?.Cancel();
            ctx = new Ctx();
            ctx.Register(ct);
        }

        public static void CancelAnew(ref Ctx ctx) {
            ctx?.Cancel();
            ctx?.Dispose();
            ctx = new Ctx();
        }

        public void Cancel() {
            cts?.Cancel();
            cts?.Dispose();
        }

        public void Register(Ct ct) {
            ct.Register(Cancel);
        }

        public void Dispose() {
            cts?.Dispose();
        }
    }
}
