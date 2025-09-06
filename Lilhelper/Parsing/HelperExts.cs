using System;
using Lilhelper.Parsing.SyntaxTree;

namespace Lilhelper.Parsing {
    public static class HelperExts {
        public static AstResult AsOk(this IAstNode self) => AstResult.Ok(self);

        public static bool IsIgnoreCase(this StringComparison rule) {
            const StringComparison cci = StringComparison.CurrentCultureIgnoreCase;
            const StringComparison ici = StringComparison.InvariantCultureIgnoreCase;
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            return
                rule is cci or ici or oic;
        }
    }
}
