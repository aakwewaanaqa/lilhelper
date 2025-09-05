namespace Lilhelper.Parsing.Tokens {
    public struct TokenPos : IPosition {
        public int Pos { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        public override string ToString() {
            return $"{Line}:{Column}";
        }

        public TokenDim CurrentNoLength() {
            return new TokenDim {
                start = this,
                end   = this
            };
        }

        public static TokenPos New() {
            return new TokenPos {
                Pos    = 0,
                Line   = 1,
                Column = 1,
            };
        }
    }
}
