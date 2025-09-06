using Lilhelper.Parsing.SyntaxTree;
using NUnit.Framework;
using static Lilhelper.Parsing.Tokens.TokenizeUnits;
using static NUnit.Framework.Assert;

namespace Lilhelper.Tests {
    public class ParsingTests_Ast {
        [Test]
        public void AsAst_Ok() {
            // Arrange
            var parser = new AstParser("1");
            var pipe = DoChar('1').AsAst();
            // Act
            var result = pipe(ref parser);
            // Assert
            That(result.IsOk);
            var astNode = result.node as TokenAstNode;
            That(astNode, Is.Not.Null);
            That(astNode, Is.TypeOf<TokenAstNode>());
            That(astNode.token.content, Is.EqualTo("1"));
        }
    }
}
