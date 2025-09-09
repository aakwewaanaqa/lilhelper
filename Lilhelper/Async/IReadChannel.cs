using System;
using System.Collections;

namespace Lilhelper.Async {
    /// <summary>
    /// 通道讀取器
    /// Defines the interface for reading elements from a channel asynchronously or synchronously.
    /// </summary>
    /// <typeparam name="T">
    /// 值類型
    /// The type of the elements managed by the channel.
    /// </typeparam>
    public interface IReadChannel<T> {
        /// <summary>
        /// 非同步讀取：若暫無元素則會等待直到有可用元素或逾時，之後以回調回傳。
        /// Asynchronous read: waits until an element is available or times out, then invokes the callback with the value.
        /// </summary>
        /// <param name="onVal">取得值後的回調。Callback invoked with the retrieved value.</param>
        /// <param name="timeout">等待的逾時秒數。Timeout in seconds for waiting.</param>
        /// <returns>可在 Unity 協程中使用的列舉器。Enumerator usable in Unity coroutines.</returns>
        IEnumerator Read(Action<T> onVal, double timeout);

        /// <summary>
        /// 當前通道內的元素數量。
        /// Current number of elements stored in the channel.
        /// </summary>
        int Length { get; }
        
        /// <summary>
        /// 這個不會 yield 沒有元素就是 false
        /// This will not yield if no element is available.
        /// </summary>
        /// <param name="onVal">值回調</param>
        bool TryRead(Action<T> onVal);
        
        /// <summary>
        /// 這個不會 yield 沒有元素就是 false
        /// This will not yield if no element is available.
        /// </summary>
        bool TryRead(out T result);
        
        /// <summary>
        /// 清除所有的寫入的值
        /// Clear all the written values.
        /// </summary>
        void Clear();
    }
}
