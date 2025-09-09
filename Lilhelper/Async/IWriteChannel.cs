namespace Lilhelper.Async {
    /// <summary>
    /// 通道寫入器：提供寫入元素並暴露取消內容。
    /// Channel writer: provides a way to write values and exposes the cancellation context.
    /// </summary>
    /// <typeparam name="T">要寫入的資料型別。Type of values to write.</typeparam>
    public interface IWriteChannel<in T> {
        /// <summary>
        /// 取消內容，用於在寫入前檢查/連動取消狀態。
        /// Cancellation context used to check/propagate cancellation before writes.
        /// </summary>
        Ctx Ctx { get; }
        /// <summary>
        /// 寫入一筆資料到通道。
        /// Write a single value into the channel.
        /// </summary>
        /// <param name="val">要寫入的值。The value to write.</param>
        void Write(T val);
        
        /// <summary>
        /// 當前通道內的元素數量。
        /// Current number of elements stored in the channel.
        /// </summary>
        int Length { get; }
    }
}
