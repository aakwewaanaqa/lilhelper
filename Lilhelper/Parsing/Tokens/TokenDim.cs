namespace Lilhelper.Parsing.Tokens {
    public struct TokenDim {
        public TokenPos start;
        public TokenPos end;

        public int Length => end.Pos - start.Pos;

        public override string ToString() {
            return $"{start} ~ {end}";
        }
    }
}
