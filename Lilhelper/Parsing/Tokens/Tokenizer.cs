using System;

namespace Lilhelper.Parsing.Tokens {
    public ref struct Tokenizer {
        private readonly ReadOnlySpan<char> src;
        private          TokenPos           pos;

        public TokenPos Pos {
            get => pos;
            set => pos = value;
        }

        public bool IsEof() =>
            pos.Pos < 0
         || pos.Pos >= src.Length;

        public Tokenizer(string src) {
            this.src = src.AsSpan();
            pos      = TokenPos.New();
        }

        private char Head => src[pos.Pos];

        public bool TryHead(out char c) {
            c = '\0';

            if (pos.Pos >= src.Length) return false;

            c = src[pos.Pos];
            return true;
        }

        public bool TryPeek(int count, out ReadOnlySpan<char> head) {
            head = string.Empty;
            if (pos.Pos < 0
             || pos.Pos         > src.Length
             || pos.Pos + count > src.Length)
                return false;
            head = src.Slice(pos.Pos, count);
            return true;
        }

        public TokenDim Advance() {
            var start = pos;
            pos += Head;
            return new TokenDim {
                start = start,
                end   = pos
            };
        }

        public TokenDim Advance(int count) {
            var dim = pos.NoLength;
            for (int i = 0; i < count; i++) {
                pos += Head;
            }

            dim.end = pos;
            return dim;
        }
    }
}
