using System;
using System.Collections;
using Lilhelper.Async;
using Lilhelper.Objs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lilhelper.AssetsProto {
    /// <summary>
    /// EN: Asset loader prototype for Unity Resources, parameterized by asset type T.
    /// ZH: 使用 Unity Resources 載入資產的原型，依 T 指定資產型別。
    /// </summary>
    /// <typeparam name="T">EN: Asset type derived from UnityEngine.Object. ZH: 繼承自 UnityEngine.Object 的資產型別。</typeparam>
    public class ResourceProto<T> : IAssetProto<T> where T : Object {
        /// <summary>
        /// EN: Internal write channel used to output loaded assets. ZH: 用於輸出已載入資產的內部寫入通道。
        /// </summary>
        private IWriteChannel<T> ch;

        /// <summary>
        /// EN: Load an asset from Resources by path and write it to the provided channel.
        /// ZH: 依路徑從 Resources 載入資產，並寫入到提供的通道。
        /// </summary>
        /// <param name="path">EN: Path under Resources. ZH: Resources 下的資產路徑。</param>
        /// <param name="wc">EN: Channel to write loaded asset. ZH: 用於輸出已載入資產的通道。</param>
        /// <returns>EN: Enumerator for Unity coroutines. ZH: 可供 Unity 協程使用的列舉器。</returns>
        public IEnumerator Load(string path, IWriteChannel<T> wc) {
            ch = wc;
            var asset = Resources.Load<T>(path);
            // EN: If asset is missing, throw to surface configuration errors early.
            // ZH: 若找不到資產，拋出例外以早期揭露設定錯誤。
            if (asset.IsNull()) throw new NullReferenceException();
            wc.Write(asset);
            yield break;
        }
    }
}
