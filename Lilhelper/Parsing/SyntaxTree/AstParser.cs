using System;
using Lilhelper.Parsing.Tokens;

namespace Lilhelper.Parsing.SyntaxTree {
    public ref struct AstParser {
        public Tokenizer Tokenizer { get; }
        public AstCtx    AstCtx    { get; }

        public AstParser(string src) {
            Tokenizer = new Tokenizer(src);
            AstCtx    = new AstCtx();
        }
    }
}
