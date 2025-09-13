using System.Collections;
using Lilhelper.Async;

namespace Lilhelper.AssetsProto {
    public class ValProto<T> : IAssetProto<T> {
        private T val;

        public ValProto(T val) {
            this.val = val;
        }

        public IAssetProto<T> SetPath(string path) {
            return this;
        }

        public string GetPath() {
            return val.ToString();
        }

        public IEnumerator Load(IWriteChannel<T> wc) {
            wc.Write(val);
            yield break;
        }
    }
}
