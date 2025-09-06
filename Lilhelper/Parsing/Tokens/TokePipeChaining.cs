using System;
using System.Text;
using Lilhelper.Async;

namespace Lilhelper.Parsing.Tokens {
    public static class TokePipeChaining {
        public static TokenPipe Or(this TokenPipe a, TokenPipe b) {
            return Pipe;

            TokenResult Pipe(ref Tokenizer self) {
                var pos = self.Pos;
                var ofA = a(ref self);
                if (ofA.IsErr) {
                    self.Pos = pos;
                    return b(ref self);
                }
                return ofA;
            }
        }

        public static TokenPipe Then(this TokenPipe a, TokenPipe b) {
            return Pipe;

            TokenResult Pipe(ref Tokenizer self) {
                var pos = self.Pos;
                var ofA = a(ref self);
                if (ofA.IsErr) {
                    self.Pos = pos;
                    return ofA;
                }
                return b(ref self);
            }
        }

        public static TokenPipe Any0(this TokenPipe a) {
            return Pipe;

            TokenResult Pipe(ref Tokenizer self) {
                var pos = self.Pos;
                var ofA = a(ref self);
                if (ofA.IsOk) return ofA;

                self.Pos = pos;
                return new TokenResult {
                    token = ofA.token,
                };
            }
        }

        public static TokenPipe Many0(this TokenPipe a) {
            return Pipe;

            TokenResult Pipe(ref Tokenizer self) {
                var builder = new StringBuilder();
                var pos   = self.Pos;
                for (;;) {
                    if (self.IsEof()) break; 
                    var ofA = a(ref self);
                    if (ofA.IsErr) break;
                    
                    builder.Append(ofA.token.content);
                }

                return new TokenResult {
                    token = new Token {
                        content   = builder.ToString(),
                        dimension = new TokenDim {
                            start = pos,
                            end   = self.Pos
                        }
                    }
                };
            }
        }

        public static TokenPipe Many1(this TokenPipe a, string supposed = "") {
            return Pipe;

            TokenResult Pipe(ref Tokenizer self) {
                var pos       = self.Pos;
                var pipeMany0 = a.Many0();
                var ofMany0   = pipeMany0(ref self);
                if (ofMany0.IsErr) {
                    self.Pos = pos;
                    return Error.ExpectationFail(supposed, pos);
                }
                return ofMany0;
            }
        }

        public static TokenPipe Extract(this TokenPipe a, IWriteChannel<Token> writeCh) {
            return Pipe;

            TokenResult Pipe(ref Tokenizer self) {
                var ofA = a(ref self);
                writeCh.Write(ofA.token);
                return ofA;
            }
        }
    }
}
