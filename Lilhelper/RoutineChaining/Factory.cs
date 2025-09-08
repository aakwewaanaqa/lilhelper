using System.Collections;
using Lilhelper.Async;

namespace Lilhelper.RoutineChaining {
    /// <summary>
    /// 協程工廠：接收通道以傳遞 <see cref="RoutineResult"/>，並回傳 IEnumerator 以供外部執行。
    /// Coroutine factory: takes a channel to communicate <see cref="RoutineResult"/> and returns IEnumerator to run.
    /// </summary>
    /// <param name="medium">用於傳遞結果或錯誤的通道。Channel used to pass results or errors.</param>
    public delegate IEnumerator Factory(Channel<RoutineResult> medium);
}
