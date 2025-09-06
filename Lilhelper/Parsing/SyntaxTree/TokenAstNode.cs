using System;
using Lilhelper.Parsing.Tokens;

namespace Lilhelper.Parsing.SyntaxTree {
    public class TokenAstNode : IAstNode {
        private const StringComparison OIC = StringComparison.OrdinalIgnoreCase;

        public Token token;

        public bool IsLikelyAs(string name) {
            return string.Equals(token.content, name, OIC);
        }
    }
}
