using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UniTasks
{
    public static class PipeExts
    {
        /// <summary>
        ///     執行<see cref="f"/>成功後執行<see cref="onOk"/>
        /// </summary>
        /// <param name="f">自己</param>
        /// <param name="onOk">成功之後的函式</param>
        /// <returns>連接起來的函式</returns>
        public static Pipe Then(this Pipe f, Pipe onOk)
        {
            return async () =>
            {
                try
                {
                    var r = await f();
                    if (r.IsFaulty) throw r.Ex;
                    return new PipeReturn(null, onOk);
                }
                catch (Exception e) { return new PipeReturn(e); }
            };
        }

        /// <summary>
        ///     執行<see cref="f"/>成功後執行<see cref="onOk"/>，或失敗時執行<see cref="onNg"/>
        /// </summary>
        /// <param name="f">自己</param>
        /// <param name="onOk">成功之後的函式</param>
        /// <param name="onNg">失敗之後的函式</param>
        /// <returns>連接起來的函式</returns>
        public static Pipe Then(this Pipe f, Pipe onOk, Pipe onNg)
        {
            return async () =>
            {
                try
                {
                    var r = await f();
                    if (r.IsFaulty) throw r.Ex;
                    return new PipeReturn(null, onOk);
                }
                catch (Exception e) { return new PipeReturn(null, onNg); }
            };
        }

        /// <summary>
        ///     嘗試執行<see cref="f"/>直到成功後執行<see cref="onOk"/>
        /// </summary>
        /// <param name="f">自己</param>
        /// <param name="onOk">成功之後的函式</param>
        /// <returns>連接起來的函式</returns>
        public static Pipe RetryThen(this Pipe f, Pipe onOk)
        {
            return async () =>
            {
                try
                {
                    var r = await f();
                    if (r.IsFaulty) throw r.Ex;
                    return new PipeReturn(null, onOk);
                }
                catch { return new PipeReturn(null, f.RetryThen(onOk)); }
            };
        }

        /// <summary>
        ///     同時執行<see cref="f"/>和<see cref="g"/>，傳回其中有錯誤的結果
        /// </summary>
        /// <returns>其中有錯誤的結果</returns>
        public static Pipe All(this Pipe f, Pipe g)
        {
            return async () =>
            {
                try
                {
                    var tuple = await UniTask.WhenAll(f(), g());
                    if (tuple.Item1.IsFaulty) throw tuple.Item1.Ex;
                    if (tuple.Item2.IsFaulty) throw tuple.Item2.Ex;
                    return default;
                }
                catch (Exception e) { return new PipeReturn(e); }
            };
        }

        /// <summary>
        ///     執行<see cref="g"/>不等待後執行<see cref="f"/>
        /// </summary>
        /// <param name="f">等待的本體</param>
        /// <param name="g">同步地執行</param>
        /// <returns><see cref="f"/>的執行結果</returns>
        public static Pipe With(this Pipe f, Pipe g)
        {
            return async () =>
            {
                try
                {
                    g().Forget();
                    var r = await f();
                    if (r.IsFaulty) throw r.Ex;
                    return default;
                }
                catch (Exception e) { return new PipeReturn(e); }
            };
        }

        /// <summary>
        ///     執行<see cref="f"/>直到沒有後續為止
        /// </summary>
        /// <param name="f">執行個體</param>
        /// <returns><see cref="f"/>的執行結果</returns>
        public static async UniTask<PipeReturn> Engage(this Pipe f)
        {
            var r              = await f();
            while (!r.IsEnd) r = await r.Continue();
            return r;
        }

        public static async UniTask LerpTo(
            this float        from,
            float             to,
            Action<float>     onF,
            float             speed = 1f,
            CancellationToken ct    = default)
        {
            ct.ThrowIfCancellationRequested();

            if (onF is null) return;

            while (!from.IsApproximately(to))
            {
                if (ct.IsCancellationRequested) return;
                await UniTask.Yield();
                from = Mathf.Lerp(from, to, speed * Time.deltaTime);
                onF(from);
            }
            onF(to);
        }

        private static bool IsApproximately(this float a, float b, float epsilon = 0.001f)
        {
            return Mathf.Abs(a - b) < epsilon;
        }

        public static async UniTask LerpTo(
            this int          from,
            int               to,
            Action<int>       onF,
            float             speed = 1f,
            CancellationToken ct    = default)
        {
            var toInt = from < to ? new Func<float, int>(Mathf.CeilToInt) : Mathf.FloorToInt;
            var f     = (float)from;
            var t     = (float)to;
            await f.LerpTo(t, v =>
            {
                onF(toInt(v));
            }, speed, ct);
        }

        public static U Apply<T, U>(this T t, Func<T, U> f)
        {
            return f(t);
        }

        public static void Let<T>(this T t, Action<T> f)
        {
            f(t);
        }
    }
}
