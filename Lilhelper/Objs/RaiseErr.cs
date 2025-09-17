using Err = System.Exception;

namespace Lilhelper.Objs {
    public static class RaiseErr {
        public static void NullRef(string msg)            => throw new System.NullReferenceException(msg);
        public static void NullRef(string msg, Err inner) => throw new System.NullReferenceException(msg, inner);
        
        public static void Todo() => throw new System.NotImplementedException();
        public static void Todo(string msg) => throw new System.NotImplementedException(msg);
        
        public static void BadOp(string msg) => throw new System.InvalidOperationException(msg);
    }
}
