namespace Lilhelper.Async {
    public interface IWriteChannel<in T> {
        Ctx Ctx { get; }
        void Write(T val);
    }
}
