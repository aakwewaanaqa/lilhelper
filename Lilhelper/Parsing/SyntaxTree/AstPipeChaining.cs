using Lilhelper.Async;
using Lilhelper.Parsing.Tokens;

namespace Lilhelper.Parsing.SyntaxTree {
    public static class AstPipeChaining {
        public static AstPipe Or(this AstPipe a, AstPipe b) {
            return Pipe;

            AstResult Pipe(ref AstParser self) {
                var ofA = a(ref self);
                return ofA.IsOk ? ofA : b(ref self);
            }
        }

        public static AstPipe Then(this AstPipe a, AstPipe b, string supposed = null) {
            return Pipe;

            AstResult Pipe(ref AstParser self) {
                var pos = self.tokenizer.Pos;
                var ofA = a(ref self);
                if (ofA.IsErr)
                    return Error.ExpectationFail(
                        $"{supposed} on then()", pos, ofA.err);
                return b(ref self);
            }
        }

        public static AstPipe Any0(this AstPipe a) {
            return Pipe;

            AstResult Pipe(ref AstParser self) {
                var ofA = a(ref self);
                if (ofA.IsErr) return AstResult.Ok(null);
                return AstResult.Ok(ofA.node);
            }
        }

        public static AstPipe Extract(this AstPipe a, IWriteChannel<IAstNode> writeCh) {
            return Pipe;

            AstResult Pipe(ref AstParser self) {
                var ofA = a(ref self);
                if (ofA.IsOk) writeCh.Write(ofA.node);
                else writeCh.Write(null);
                return ofA;
            }
        }

        public static AstPipe AsAst(this TokenPipe a, string supposed = "") {
            return Pipe;

            AstResult Pipe(ref AstParser self) {
                var pos = self.tokenizer.Pos;
                var ofA = a(ref self.tokenizer);
                if (ofA.IsErr)
                    return Error.ExpectationFail(
                        $"{supposed} on then()", pos, ofA.err);

                return new AstResult();
            }
        }
    }
}
