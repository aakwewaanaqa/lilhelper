namespace Lilhelper.Parsing {
    public interface IPosition {
        public int Pos    { get; set; }
        public int Line   { get; set; }
        public int Column { get; set; }
    }
}
