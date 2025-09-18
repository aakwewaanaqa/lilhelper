using Lilhelper.Async;

using UnityEngine;

using Err = System.Exception;

namespace Lilhelper.Objs {
    public static class RaiseErr {
        public static void NullRef(string msg)            => throw new System.NullReferenceException(msg);
        public static void NullRef(string msg, Err inner) => throw new System.NullReferenceException(msg, inner);

        public static void Todo()           => throw new System.NotImplementedException();
        public static void Todo(string msg) => throw new System.NotImplementedException(msg);

        public static void BadOp(string msg) => throw new System.InvalidOperationException(msg);

        public static void IndexErr(int index) => throw new System.IndexOutOfRangeException(index.ToString());

        public static void NoVal(string msg = null) {
            var exp = new ChannelNoValueException(msg, null);
            Debug.LogException(exp);
            throw exp;
        }
    }
}
