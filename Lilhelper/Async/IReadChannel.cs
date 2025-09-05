using System;
using System.Collections;

namespace Lilhelper.Async {
    public interface IReadChannel<out T> {
        IEnumerator Read(Action<T> onVal);
    }
}
