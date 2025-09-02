using System;
using Ct = System.Threading.CancellationToken;

namespace Lilhelper.Objs {
    public static class CtExts {
        public static void ThrowIfCancel(this Ct ct, Action onThrow = null) {
            if (!ct.IsCancellationRequested) return;

            onThrow?.Invoke();
            ct.ThrowIfCancellationRequested();
        }
    }
}
