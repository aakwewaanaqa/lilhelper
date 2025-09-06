using Lilhelper.Parsing.Tokens;
using NUnit.Framework;
using static Lilhelper.Parsing.Tokens.TokenizeUnits;

namespace Tests {
    public partial class ParsingTests {
        [Test]
        public void A_Then_B() {
            var tokenizer = new Tokenizer("ab");
            var pos       = tokenizer.Pos;
            var ofA       = DoChar('a')(ref tokenizer);
            Assert.That(ofA.IsOk);
            
            pos = tokenizer.Pos;
            var ofB = DoChar('b')(ref tokenizer);
            Assert.That(ofB.token.dimension, Is.EqualTo(new TokenDim {
                start = pos,
                end   = pos + 'b' 
            }));
            Assert.That(ofB.IsOk);
        }

        [Test]
        public void DoChar_Failure_DoesNotAdvance() {
            var tokenizer = new Tokenizer("x");
            var pos0 = tokenizer.Pos;
            var res = DoChar('a')(ref tokenizer);
            Assert.That(res.IsErr, Is.True);
            Assert.That(tokenizer.Pos, Is.EqualTo(pos0));
        }

        [Test]
        public void DoChar_Range_Digit_Succeeds() {
            var tokenizer = new Tokenizer("7");
            var pos0 = tokenizer.Pos;
            var res = DoChar('0', '9')(ref tokenizer);
            Assert.That(res.IsOk, Is.True);
            Assert.That(res.token.content, Is.EqualTo("7"));
            Assert.That(res.token.dimension, Is.EqualTo(new TokenDim {
                start = pos0,
                end   = pos0 + '7'
            }));
            Assert.That(res.token.Length, Is.EqualTo(1));
        }

        [Test]
        public void DoString_IgnoreCase_Succeeds_And_Keeps_Source_Casing() {
            var tokenizer = new Tokenizer("HelloX");
            var pos0 = tokenizer.Pos;
            var res = DoString("heLLo")(ref tokenizer); // default OrdinalIgnoreCase
            Assert.That(res.IsOk, Is.True);
            Assert.That(res.token.content, Is.EqualTo("Hello")); // should keep original casing from source when ignore case
            Assert.That(res.token.dimension.Length, Is.EqualTo(5));
            Assert.That(tokenizer.Pos.Pos - pos0.Pos, Is.EqualTo(5));
            // Next char should remain 'X'
            Assert.That(DoChar('X')(ref tokenizer).IsOk, Is.True);
        }

        [Test]
        public void DoString_Ordinal_Fails_And_DoesNotAdvance() {
            var tokenizer = new Tokenizer("Hello");
            var pos0 = tokenizer.Pos;
            var res = DoString("HELLO", System.StringComparison.Ordinal)(ref tokenizer);
            Assert.That(res.IsErr, Is.True);
            Assert.That(tokenizer.Pos, Is.EqualTo(pos0));
        }

        [Test]
        public void GreedEnds_Succeeds_And_Leaves_Delimiter_For_Next_Parse() {
            var tokenizer = new Tokenizer("abc;X");
            var pos0 = tokenizer.Pos;
            var res = GreedEnds(";")(ref tokenizer);
            Assert.That(res.IsOk, Is.True);
            Assert.That(res.token.content, Is.EqualTo("abc"));
            Assert.That(res.token.dimension.Length, Is.EqualTo(3));
            // Should have advanced only the content length, not including delimiter
            Assert.That(tokenizer.Pos.Pos - pos0.Pos, Is.EqualTo(3));

            // Now the next char should be the delimiter ';'
            var pos1 = tokenizer.Pos;
            var semi = DoChar(';')(ref tokenizer);
            Assert.That(semi.IsOk, Is.True);
            Assert.That(semi.token.dimension, Is.EqualTo(new TokenDim {
                start = pos1,
                end   = pos1 + ';'
            }));
        }

        [Test]
        public void GreedEnds_Fails_When_No_Delimiter() {
            var tokenizer = new Tokenizer("abcX");
            var pos0 = tokenizer.Pos;
            var res = GreedEnds(";")(ref tokenizer);
            Assert.That(res.IsErr, Is.True);
            Assert.That(tokenizer.Pos, Is.EqualTo(pos0));
        }

        [Test]
        public void DoChar_NewLine_Advances_Line_And_Resets_Column() {
            var tokenizer = new Tokenizer("\n");
            var pos0 = tokenizer.Pos;
            var res = DoChar('\n')(ref tokenizer);
            Assert.That(res.IsOk, Is.True);
            Assert.That(res.token.dimension.start, Is.EqualTo(pos0));
            var expectedEnd = new TokenPos { Pos = pos0.Pos + 1, Line = pos0.Line + 1, Column = 1 };
            Assert.That(res.token.dimension.end, Is.EqualTo(expectedEnd));
        }

        [Test]
        public void EmptyInput_Parsers_Fail_Without_Advancing() {
            var tokenizer = new Tokenizer(string.Empty);
            var pos0 = tokenizer.Pos;

            var a = DoChar('a')(ref tokenizer);
            Assert.That(a.IsErr, Is.True);
            Assert.That(tokenizer.Pos, Is.EqualTo(pos0));

            var s = DoString("abc")(ref tokenizer);
            Assert.That(s.IsErr, Is.True);
            Assert.That(tokenizer.Pos, Is.EqualTo(pos0));

            var g = GreedEnds(";")(ref tokenizer);
            Assert.That(g.IsErr, Is.True);
            Assert.That(tokenizer.Pos, Is.EqualTo(pos0));
        }
    }
}
