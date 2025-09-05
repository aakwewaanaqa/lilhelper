using System;

namespace Lilhelper.Parsing.Tokens {
    public ref struct Tokenizer {
        private readonly ReadOnlySpan<char> src;
        private          TokenPos           pos;

        public TokenPos Pos => pos;

        public bool IsEof() =>
            pos.Pos < 0
         || pos.Pos >= src.Length;

        public bool IsEof(int count) =>
            pos.Pos         < 0
         || pos.Pos         >= src.Length
         || pos.Pos + count >= src.Length;


        public Tokenizer(string src) {
            this.src = src.AsSpan();
            pos      = TokenPos.New();
        }

        public bool TryHead(out char c) {
            c = '\0';

            if (IsEof()) return false;

            c = src[pos.Pos];
            return true;
        }

        public bool TryPeek(int count, out ReadOnlySpan<char> head) {
            head = string.Empty;
            if (IsEof(count)) return false;
            head = src.Slice(pos.Pos, count);
            return true;
        }

        private TokenPos Alter(TokenPos it) {
            if (!TryHead(out char head)) return it;

            if (head is '\n' or '\r') {
                it.Pos++;
                it.Line++;
                it.Column = 1;
                return it;
            }

            it.Pos++;
            it.Column++;
            return it;
        }

        public TokenDim Advance() {
            var start = pos;
            pos = Alter(pos);
            return new TokenDim {
                start = start,
                end   = pos
            };
        }

        public TokenDim Advance(int count) {
            var dim = pos.CurrentNoLength();
            for (int i = 0; i < count; i++) {
                dim.end = Alter(dim.end);
            }

            return dim;
        }
    }
}
