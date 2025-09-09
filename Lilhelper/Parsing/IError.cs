namespace Lilhelper.Parsing {
    public interface IError {
        ErrorCode Code  { get; }
        string    Msg   { get; }
        IPosition Pos   { get; }
        Error     Inner { get; }
        Error     SetInner(Error inner);
    }
}
