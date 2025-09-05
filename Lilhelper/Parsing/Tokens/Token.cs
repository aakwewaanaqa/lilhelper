using System;

namespace Lilhelper.Parsing.Tokens {
    public struct Token {
        public string   content;
        public TokenDim dimension;

        public int Length => dimension.Length;

        public override string ToString() {
            return $"{content.ToString()}({dimension.ToString()})";
        }
    }
}
