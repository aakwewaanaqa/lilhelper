using System;

namespace Lilhelper.Parsing.Tokens {
    public ref struct Tokenizer {
        private readonly ReadOnlySpan<char> src;
        private          TokenPos           pos;

        public TokenPos Pos {
            readonly get => pos.Clone();
            private set => pos = value;
        }

        public bool IsEof() =>
            Pos.Pos < 0
         || Pos.Pos >= src.Length;

        public bool IsEof(int count) =>
            Pos.Pos         < 0
         || Pos.Pos         >= src.Length
         || Pos.Pos + count >= src.Length;


        public Tokenizer(string src) {
            this.src = src.AsSpan();
            pos      = TokenPos.New();
        }

        public bool TryHead(out char c) {
            c = '\0';

            if (IsEof()) return false;

            c = src[Pos.Pos];
            return true;
        }

        public bool TryPeek(int count, out ReadOnlySpan<char> head) {
            head = string.Empty;
            if (IsEof(count)) return false;
            head = src.Slice(Pos.Pos, count);
            return true;
        }

        private void AlterPos() {
            if (!TryHead(out char head)) return;

            if (head is '\n' or '\r') {
                pos.Pos++;
                pos.Line++;
                pos.Column = 1;
                return;
            }

            pos.Pos++;
            pos.Column++;
        }

        public TokenDim Advance() {
            var start = Pos;
            AlterPos();
            return new TokenDim {
                start = start,
                end   = Pos
            };
        }

        public TokenDim Advance(int count) {
            var dim = Pos.NoLength;
            for (int i = 0; i < count; i++) {
                AlterPos();
            }
            dim.end = Pos;
            return dim;
        }
    }
}
