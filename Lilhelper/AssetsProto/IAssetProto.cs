using System.Collections;

using Lilhelper.Async;

namespace Lilhelper.AssetsProto {
    public interface IAssetProto<out T> {
        IAssetProto<T> SetPath(string path);
        string         GetPath();
        IEnumerator    Load(IWriteChannel<T> wc);
    }
}
