using System;

namespace Lilhelper.RoutineChaining {
    /// <summary>
    /// 協程步驟結果：包含標籤、錯誤與下一步。
    /// Result of a coroutine step: carries label, error, and the next step.
    /// </summary>
    public struct RoutineResult {
        /// <summary>
        /// 步驟標籤。
        /// Label for the step.
        /// </summary>
        public string label;
        /// <summary>
        /// 錯誤（非空代表失敗）。
        /// Error; non-null indicates failure.
        /// </summary>
        public Exception ex;
        /// <summary>
        /// 下一個要執行的步驟。
        /// Next step to run.
        /// </summary>
        public Factory next;

        /// <summary>
        /// <see cref="ex"/> 非空。
        /// <see cref="ex"/> is not null.
        /// </summary>
        public bool IsErr   => ex is not null;
        /// <summary>
        /// <see cref="ex"/> 為空。
        /// <see cref="ex"/> is null.
        /// </summary>
        public bool IsOk    => ex is null;
        /// <summary>
        /// <see cref="next"/> 為空。
        /// <see cref="next"/> is null.
        /// </summary>
        public bool IsEnd   => next is null;
        /// <summary>
        /// <see cref="next"/> 非空。
        /// <see cref="next"/> is not null.
        /// </summary>
        public bool HasNext => next is not null;

        /// <summary>
        /// 以訊息建立錯誤結果（可選標籤）。
        /// Create an error result with a message (optional label).
        /// </summary>
        /// <param name="msg">錯誤訊息。Error message.</param>
        /// <param name="label">步驟標籤。Optional label.</param>
        public static RoutineResult Err(string msg, string label = null) =>
            new() {
                ex    = new Exception(msg),
                label = label
            };
    }
}
