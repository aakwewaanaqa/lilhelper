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
        /// 如果通道沒有元素會 yield 有的話在 <see cref="onVal"/> 中回調
        /// If no element, yield and return in <see cref="onVal"/>
        /// </summary>
        /// <param name="onVal">值回調</param>
        /// <param name="timeout">避免超時的時間限制 avoid overtime waiting on this function</param>
        IEnumerator Read(Action<T> onVal, double timeout);

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
