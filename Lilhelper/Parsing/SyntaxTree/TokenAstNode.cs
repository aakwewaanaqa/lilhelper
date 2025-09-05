using System;
using Lilhelper.Parsing.Tokens;

namespace Lilhelper.Parsing.SyntaxTree {
    public class TokenAstNode : IAstNode {
        public Token token;

        public bool IsLikelyAs(string name) {
            const StringComparison OPT = StringComparison.InvariantCultureIgnoreCase;
            return string.Equals(token.content, name, OPT);
        }
    }
}
