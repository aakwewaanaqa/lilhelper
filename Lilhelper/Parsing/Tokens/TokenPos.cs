namespace Lilhelper.Parsing.Tokens {
    public class TokenPos : IPosition {
        public int Pos { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        public TokenPos Clone() =>
            new() {
                Pos    = Pos,
                Line   = Line,
                Column = Column,
            };

        public override string ToString() {
            return $"{Line}:{Column}";
        }

        public TokenDim NoLength =>
            new() {
                start = this,
                end   = this
            };

        public static TokenPos New() {
            return new TokenPos {
                Pos    = 0,
                Line   = 1,
                Column = 1,
            };
        }
    }
}
