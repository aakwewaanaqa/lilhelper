using System;

namespace Lilhelper.Parsing.Tokens {
    public static class TokenizeUnits {
        public static TokenPipe DoChar(char c) => self => {
            if (!self.TryHead(out char head) || head != c)
                return TokenResult.Err($"expected {c}", self.Pos);

            var content = head.ToString();
            return new TokenResult {
                token = new Token {
                    content   = content,
                    dimension = self.Advance()
                }
            };
        };

        public static TokenPipe DoChar(char f, char t) => self => {
            if (!self.TryHead(out char head) || head < f || head > t)
                return TokenResult.Err($"expected char from {f} to {t}", self.Pos);

            var content = head.ToString();
            return new TokenResult {
                token = new Token {
                    content   = content,
                    dimension = self.Advance()
                }
            };
        };

        public static TokenPipe DoString(string str, StringComparison rule = StringComparison.OrdinalIgnoreCase) => self => {
            if (!self.TryPeek(str.Length, out var head)
             || string.Equals(head.ToString(), str, rule))
                return TokenResult.Err($"expected {str}", self.Pos);

            var content = head;
            return new TokenResult {
                token = new Token {
                    content   = content.ToString(),
                    dimension = self.Advance(str.Length)
                }
            };
        };

        public static TokenPipe GreedEnds(string ends) => self => {
            for (int i = 0; !self.IsEof(); i++) {
                if (!self.TryPeek(i, out var head)) break;
                if (!head.EndsWith(ends)) continue;

                var content = head[..^ends.Length];
                return new TokenResult {
                    token = new Token {
                        content   = content.ToString(),
                        dimension = self.Advance(content.Length)
                    }
                };
            }

            return TokenResult.Err($"expected ends with {ends} but not found", self.Pos);
        };
    }
}
