using System.Text;
using Lilhelper.Async;

namespace Lilhelper.Parsing.Tokens {
    public static class TokePipeChaining {
        public static TokenPipe Or(this TokenPipe a, TokenPipe b) => self => {
            var ofA = a(self);
            return ofA.IsOk ? ofA : b(self);
        };

        public static TokenPipe Then(this TokenPipe a, TokenPipe b) => self => {
            var ofA = a(self);
            if (!ofA.IsOk) return ofA;
            return b(self);
        };

        public static TokenPipe Any0(this TokenPipe a, string errTag = "") => self => {
            var pos = self.Pos;
            var ofA = a(self);
            if (ofA.IsErr) return ofA;
            return new TokenResult {
                token = new Token {
                    content = ofA.token.content,
                    dimension = new TokenDim {
                        start = pos,
                        end   = self.Pos
                    }
                }
            };
        };

        public static TokenPipe Many0(this TokenPipe a) => self => {
            var builder = new StringBuilder();
            var start   = self.Pos;
            for (;;) {
                var ofA = a(self);
                if (!ofA.IsOk) break;
                builder.Append(ofA.token.content);
            }

            return new TokenResult {
                token = new Token {
                    content = builder.ToString(),
                    dimension = new TokenDim {
                        start = start,
                        end   = self.Pos
                    }
                }
            };
        };

        public static TokenPipe Many1(this TokenPipe a, string supposed = "") => self => {
            var pos = self.Pos;
            var pipeMany0 = a.Many0();
            var ofMany0   = pipeMany0(self);
            if (ofMany0.IsNone) 
                return Error.ExpectationFail(supposed, pos);
            return ofMany0;
        };

        public static TokenPipe Extract(this TokenPipe a, IWriteChannel<Token> writeCh) => self => {
            var ofA = a(self);
            writeCh.Write(ofA.token);
            return ofA;
        };
    }
}
