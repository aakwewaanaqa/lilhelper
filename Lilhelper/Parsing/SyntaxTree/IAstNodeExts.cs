namespace Lilhelper.Parsing.SyntaxTree {
    public static class IAstNodeExts {
        public static AstResult AsOk(this IAstNode self) => AstResult.Ok(self);
    }
}
