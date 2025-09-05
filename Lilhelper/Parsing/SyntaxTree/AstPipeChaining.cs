using Lilhelper.Async;
using Lilhelper.Parsing.Tokens;

namespace Lilhelper.Parsing.SyntaxTree {
    public static class AstPipeChaining {
        public static AstPipe Or(this AstPipe a, AstPipe b) => self => {
            var ofA = a(self);
            return ofA.IsOk ? ofA : b(self);
        };

        public static AstPipe Then(this AstPipe a, AstPipe b, string supposed = null) => self => {
            var pos = self.Tokenizer.Pos;
            var ofA = a(self);
            if (ofA.IsErr)
                return AstResult.Err(
                    $"Supposed to parsed {supposed} on then() but fail",
                    pos,
                    ofA.err
                );
            return b(self);
        };

        public static AstPipe Any0(this AstPipe a) => self => {
            var pos = self.Tokenizer.Pos;
            var ofA = a(self);
            if (ofA.IsErr) return AstResult.Ok(null);
            return AstResult.Ok(ofA.node);
        };

        public static AstPipe Extract(this AstPipe a, IWriteChannel<IAstNode> writeCh) => self => {
            var ofA = a(self);
            if (ofA.IsOk) writeCh.Write(ofA.node);
            else writeCh.Write(null);
            return ofA;
        };

        public static AstPipe AsAst(this TokenPipe a, string supposed = null) => self => {
            var pos = self.Tokenizer.Pos;
            var ofA = a(self.Tokenizer);
            if (ofA.IsErr)
                return AstResult.Err(
                    $"Supposed to parsed {supposed} asAst() but fail",
                    pos,
                    ofA.err
                );

            return new AstResult();
        };
    }
}
