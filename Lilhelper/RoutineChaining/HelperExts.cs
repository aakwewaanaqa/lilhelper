using System;
using System.Collections;
using Lilhelper.Async;

namespace Lilhelper.RoutineChaining {
    /// <summary>
    /// 協程串接輔助方法。
    /// Helper extensions for chaining coroutine steps.
    /// </summary>
    public static class HelperExts {
        /// <summary>
        /// 串接下一個步驟：若前一步回傳錯誤則中止，否則把下一步資訊寫回通道。
        /// Chain to the next step: aborts on error from previous step, otherwise writes the next step into the channel.
        /// </summary>
        /// <param name="self">目前步驟。Current step.</param>
        /// <param name="next">下一個步驟。Next step to execute.</param>
        public static Factory Then(this Factory self, Factory next) {
            return Pipe;

            IEnumerator Pipe(Channel<RoutineResult> medium) {
                if (medium.TryRead(out var ofSelf)) {
                    if (ofSelf.IsErr) {
                        medium.Clear();
                        medium.Write(ofSelf);
                        yield break;
                    }
                }
                
                yield return self(medium);
                yield return medium.Read(val => ofSelf = val);
                medium.Write(new RoutineResult {
                    label = ofSelf.label,
                    ex    = ofSelf.ex,
                    next  = next,
                });
            }
        }


        /// <summary>
        /// 將工廠標記為下一步，並附上標籤。
        /// Wrap the factory as the next step with a label.
        /// </summary>
        /// <param name="self">要前往的下一步。The next factory.</param>
        /// <param name="label">步驟標籤。Step label.</param>
        public static RoutineResult AsNext(this Factory self, string label) =>
            new() {
                next  = self,
                label = label
            };

        /// <summary>
        /// 以例外建立錯誤結果。
        /// Convert an exception into an error result.
        /// </summary>
        /// <param name="ex">例外。Exception.</param>
        public static RoutineResult AsErr(this Exception ex) =>
            new() {
                ex = ex
            };
    }
}
