using System;

namespace Lilhelper.Parsing.Tokens {
    public static class TokenizeUnits {
        public static TokenPipe DoChar(char c) => self => {
            var pos = self.Pos;
            if (!self.TryHead(out char head) || head != c)
                return Error.ExpectationFail($"{c}", pos);

            string content = head.ToString();
            return new TokenResult {
                token = new Token {
                    content   = content,
                    dimension = self.Advance()
                }
            };
        };

        public static TokenPipe DoChar(char f, char t) => self => {
            var pos = self.Pos;
            if (!self.TryHead(out char head) || head < f || head > t)
                return Error.ExpectationFail($"char from {f} to {t}", pos);

            string content = head.ToString();
            return new TokenResult {
                token = new Token {
                    content   = content,
                    dimension = self.Advance()
                }
            };
        };

        public static TokenPipe DoString(string str, StringComparison rule = StringComparison.OrdinalIgnoreCase) =>
            self => {
                var pos = self.Pos;
                if (!self.TryPeek(str.Length, out var head)
             || string.Equals(head.ToString(), str, rule))
                    return Error.ExpectationFail(str, pos);

                var content = head;
                return new TokenResult {
                    token = new Token {
                        content   = content.ToString(),
                        dimension = self.Advance(str.Length)
                    }
                };
            };

        public static TokenPipe GreedEnds(string ends) => self => {
            var pos = self.Pos;
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

            return Error.ExpectationFail($"has ends {ends}", pos);
        };
    }
}
