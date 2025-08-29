using System;
using Cysharp.Threading.Tasks;

namespace UniTasks
{
    /// <summary>
    ///     流程管線
    /// </summary>
    public delegate UniTask<PipeReturn> Pipe();

    /// <summary>
    ///     流程執行結果回傳
    /// </summary>
    public readonly struct PipeReturn
    {
        /// <summary>
        ///     內部錯誤
        /// </summary>
        public Exception Ex { get; }

        /// <summary>
        ///     <see cref="Continue"/>會執行的對象
        /// </summary>
        public Pipe Then { get; }

        /// <summary>
        ///     <see cref="Ex"/> is not null
        /// </summary>
        public bool IsFaulty => Ex is not null;

        /// <summary>
        ///     <see cref="Then"/> is null
        /// </summary>
        public bool IsEnd => Then is null;

        public PipeReturn(Exception ex = null, Pipe then = null)
        {
            Ex   = ex;
            Then = then;
        }

        public static PipeReturn Fail(Exception ex) => new(ex);

        public async UniTask<PipeReturn> Continue()
        {
            return await Then();
        }
    }
}
