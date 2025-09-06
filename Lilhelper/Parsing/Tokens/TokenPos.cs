namespace Lilhelper.Parsing.Tokens {
    public struct TokenPos : IPosition {
        public int Pos { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

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

        public static TokenPos operator +(TokenPos p, char c) {
            return c switch {
                '\r' or '\n' => new TokenPos {
                    Pos    = p.Pos  + 1,
                    Line   = p.Line + 1,
                    Column = 1
                },
                _ => new TokenPos {
                    Pos    = p.Pos + 1,
                    Line   = p.Line,
                    Column = p.Column + 1
                }
            };
        }
    }
}
