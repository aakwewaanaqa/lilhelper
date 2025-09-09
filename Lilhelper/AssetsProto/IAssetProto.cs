using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using Lilhelper.Async;

namespace Lilhelper.AssetsProto {
    public interface IAssetProto<out T> {
        IEnumerator Load(string path, IWriteChannel<T> wc);
    }
}
