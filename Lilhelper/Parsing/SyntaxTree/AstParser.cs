using System;
using Lilhelper.Parsing.Tokens;

namespace Lilhelper.Parsing.SyntaxTree {
    public ref struct AstParser {
        public Tokenizer tokenizer;
        public AstCtx    astCtx;

        public AstParser(string src) {
            tokenizer = new Tokenizer(src);
            astCtx    = new AstCtx();
        }
    }
}
