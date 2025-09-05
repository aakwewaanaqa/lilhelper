namespace Lilhelper.Parsing.SyntaxTree {
    public struct AstResult {
        public IAstNode node;
        public Error    err;

        public static AstResult Err(string msg, IPosition pos, Error inner = null) {
            return new AstResult {
                err = new Error(msg, pos, inner)
            };
        }

        public static AstResult Ok(IAstNode node) => new() {
            node = node
        };

        public bool IsErr => err is not null;

        public bool IsOk => err is null;
    }
}
