using System;

namespace Lilhelper.Parsing {
    public ref struct Parser {
        private readonly ReadOnlySpan<char> src;
        private          int                pos;

        public Parser(string src) {
            pos      = 0;
            this.src = src.AsSpan();
        }
    }
}
