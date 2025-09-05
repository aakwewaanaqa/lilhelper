namespace Lilhelper.Parsing.SyntaxTree {
    public struct AstResult {
        public IAstNode node;
        public Error    err;

        public static AstResult Err(Error outer, Error inner = null) {
            return new AstResult {
                err = outer.SetInner(inner)
            };
        }

        public static AstResult Ok(IAstNode node) => new() {
            node = node
        };

        public bool IsErr => err is not null;

        public bool IsOk => err is null;
        
        public static implicit operator AstResult(Error err) => Err(err);
    }
}
