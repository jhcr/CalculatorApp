using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using NSubstitute;
using CalculatorApp.EntityService;
using System.Linq;

namespace Tests
{
    public class Lexer_UnitTests
    {
        Lexer _lexer;
        ITokenizer _tokenizerMock;

        public Lexer_UnitTests()
        {
            _tokenizerMock = Substitute.For<ITokenizer>();
            _lexer = new Lexer(_tokenizerMock);
        }

        [Fact]
        public void Should_Identify_All_Delimitors_When_Multiple_Delimitors_Given()
        {
            _tokenizerMock.TryParseDelimiter(Arg.Is("1"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("1,"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(arg => { arg[1] = ","; arg[2] = "1"; return true; });
            _tokenizerMock.Identify(Arg.Is("1")).Returns(new Token(TokenType.Number,"1",1));
            _tokenizerMock.Identify(Arg.Is(",")).Returns(new Token(TokenType.PlusOperator, ","));

            var actual = _lexer.Scan("1,1,1");

            Assert.Equal(5, actual.Count());
            var tokens = actual.ToArray();
            Assert.Equal(TokenType.Number, tokens[0].Type);
            Assert.Equal(TokenType.PlusOperator, tokens[1].Type);
            Assert.Equal(TokenType.Number, tokens[2].Type);
            Assert.Equal(TokenType.PlusOperator, tokens[3].Type);
            Assert.Equal(TokenType.Number, tokens[4].Type);

            _tokenizerMock.Received(2).TryParseDelimiter(Arg.Is("1,"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(3).TryParseDelimiter(Arg.Is("1"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.DidNotReceive().TryParseDelimiter(Arg.Is((string s) => s.Length > 2 || !s.StartsWith("1")), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(3).Identify(Arg.Is("1"));
            _tokenizerMock.Received(2).Identify(Arg.Is(","));
            _tokenizerMock.DidNotReceive().Identify(Arg.Is((string s) => s.Length > 1 || s!="1" && s!=","));

        }

        [Fact]
        public void Should_Tokens_End_With_Number_When_Source_End_With_Operator()
        {
            _tokenizerMock.TryParseDelimiter(Arg.Is("1"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("1,"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(arg => { arg[1] = ","; arg[2] = "1"; return true; });
            _tokenizerMock.Identify(Arg.Is("1")).Returns(new Token(TokenType.Number, "1", 1));
            _tokenizerMock.Identify(Arg.Is(",")).Returns(new Token(TokenType.PlusOperator, ","));
            _tokenizerMock.Identify(Arg.Is(string.Empty)).Returns(new Token(TokenType.IgnoredNumber, string.Empty));

            var actual = _lexer.Scan("1,1,");

            Assert.Equal(5, actual.Count());
            var tokens = actual.ToArray();
            Assert.Equal(TokenType.IgnoredNumber, tokens[4].Type);
            Assert.Null(tokens[4].Value);
            Assert.Equal(string.Empty, tokens[4].Raw);
        }

        [Fact]
        public void Should_Throw_NagativeNumberException_When_Negative_Number_Given()
        {
            _tokenizerMock.TryParseDelimiter(Arg.Is("1"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("1,"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(arg => { arg[1] = ","; arg[2] = "1"; return true; });
            _tokenizerMock.TryParseDelimiter(Arg.Is("-"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("-1"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("-1,"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(arg => { arg[1] = ","; arg[2] = "-1"; return true; });
            _tokenizerMock.Identify(Arg.Is("1")).Returns(new Token(TokenType.Number, "1", 1));
            _tokenizerMock.Identify(Arg.Is(",")).Returns(new Token(TokenType.PlusOperator, ","));
            _tokenizerMock.Identify(Arg.Is("-1")).Returns(new Token(TokenType.Number, "-1", -1));

            Assert.Throws<NagativeNumberException>(() => _lexer.Scan("1,-1,-1"));

            _tokenizerMock.Received(1).TryParseDelimiter(Arg.Is("1,"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(1).TryParseDelimiter(Arg.Is("-1,"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(2).Identify(Arg.Is("-1"));

        }

        [Fact]
        public void Should_Throw_NullArgumentException_When_Null_Source_Given()
        {
            _tokenizerMock.TryParseDelimiter(Arg.Is("1"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("1,"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(arg => { arg[1] = ","; arg[2] = "1"; return true; });
            _tokenizerMock.TryParseDelimiter(Arg.Is("-"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("-1"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.Identify(Arg.Is("1")).Returns(new Token(TokenType.Number, "1", 1));
            _tokenizerMock.Identify(Arg.Is(",")).Returns(new Token(TokenType.PlusOperator, ","));
            _tokenizerMock.Identify(Arg.Is("-1")).Returns(new Token(TokenType.Number, "-1", -1));

            Assert.Throws<ArgumentNullException>("source", () => _lexer.Scan(null));

            _tokenizerMock.DidNotReceiveWithAnyArgs().TryParseDelimiter(Arg.Any<string>(), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.DidNotReceiveWithAnyArgs().Identify(Arg.Any<string>());
        }

        [Fact]
        public void Should_Return_IgnoredNumber_Token_When_Empty_Source_Given()
        {
            _tokenizerMock.TryParseDelimiter(Arg.Is(""), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.Identify(Arg.Is("")).Returns(new Token(TokenType.IgnoredNumber, ""));

            var actual = _lexer.Scan("");

            Assert.Single(actual);
            var tokens = actual.ToArray();
            Assert.Equal(TokenType.IgnoredNumber, tokens[0].Type);
            Assert.Null(tokens[0].Value);
            Assert.Equal(string.Empty, tokens[0].Raw);

            _tokenizerMock.Received(0).TryParseDelimiter(Arg.Is(""), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(1).Identify(Arg.Is(""));
        }

        /* Commented out after moved custom delimiter logic to application service level
        [Fact]
        public void Should_Custom_Delimitor_Work_When_Single_Char_Given()
        {
            _tokenizerMock.TryParseDelimiter(Arg.Any<string>(), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("1a"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(arg => { arg[1] = "a"; arg[2] = "1"; return true; });
            _tokenizerMock.Identify(Arg.Is("1")).Returns(new Token(TokenType.Number, "1", 1));
            _tokenizerMock.Identify(Arg.Is("a")).Returns(new Token(TokenType.PlusOperator, "a"));

            
            var actual = _lexer.Scan($"//a\\n1a1");

            Assert.Equal(3, actual.Count());
            var tokens = actual.ToArray();
            Assert.Equal(TokenType.Number, tokens[0].Type);
            Assert.Equal(TokenType.PlusOperator, tokens[1].Type);
            Assert.Equal("a", tokens[1].Raw);
            Assert.Equal(TokenType.Number, tokens[2].Type);

            _tokenizerMock.Received(1).TryParseDelimiter(Arg.Is("1a"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(2).TryParseDelimiter(Arg.Is("1"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.DidNotReceive().TryParseDelimiter(Arg.Is((string s) => s.Length > 2 || !s.StartsWith("1")), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(2).Identify(Arg.Is("1"));
            _tokenizerMock.Received(1).Identify(Arg.Is("a"));
            _tokenizerMock.DidNotReceive().Identify(Arg.Is((string s) => s != "1" && s != "a"));

        }

        [Fact]
        public void Should_Custom_Delimitor_Work_When_Any_Length_Given()
        {
            _tokenizerMock.TryParseDelimiter(Arg.Any<string>(), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("1aaa"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(arg => { arg[1] = "aaa"; arg[2] = "1"; return true; });
            _tokenizerMock.Identify(Arg.Is("1")).Returns(new Token(TokenType.Number, "1", 1));
            _tokenizerMock.Identify(Arg.Is("aaa")).Returns(new Token(TokenType.PlusOperator, "aaa"));

            var actual = _lexer.Scan($"//[aaa]\\n1aaa1");

            Assert.Equal(3, actual.Count());
            var tokens = actual.ToArray();
            Assert.Equal(TokenType.Number, tokens[0].Type);
            Assert.Equal(TokenType.PlusOperator, tokens[1].Type);
            Assert.Equal("aaa", tokens[1].Raw);
            Assert.Equal(TokenType.Number, tokens[2].Type);

            _tokenizerMock.Received(1).TryParseDelimiter(Arg.Is("1aaa"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(2).TryParseDelimiter(Arg.Is("1"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.DidNotReceive().TryParseDelimiter(Arg.Is((string s) => s.Length > 4 || !s.StartsWith("1")), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(2).Identify(Arg.Is("1"));
            _tokenizerMock.Received(1).Identify(Arg.Is("aaa"));
            _tokenizerMock.DidNotReceive().Identify(Arg.Is((string s) => s != "1" && s != "aaa"));

        }

        [Fact]
        public void Should_Custom_Delimitor_Work_When_Multiple_Given()
        {
            _tokenizerMock.TryParseDelimiter(Arg.Any<string>(), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("1**"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(arg => { arg[1] = "**"; arg[2] = "1"; return true; });
            _tokenizerMock.TryParseDelimiter(Arg.Is("1??"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(arg => { arg[1] = "??"; arg[2] = "1"; return true; });
            _tokenizerMock.Identify(Arg.Is("1")).Returns(new Token(TokenType.Number, "1", 1));
            _tokenizerMock.Identify(Arg.Is("**")).Returns(new Token(TokenType.PlusOperator, "**"));
            _tokenizerMock.Identify(Arg.Is("??")).Returns(new Token(TokenType.PlusOperator, "??"));

            var actual = _lexer.Scan($"//[**][??]\\n1**1??1");

            Assert.Equal(5, actual.Count());
            var tokens = actual.ToArray();
            Assert.Equal(TokenType.Number, tokens[0].Type);
            Assert.Equal(TokenType.PlusOperator, tokens[1].Type);
            Assert.Equal("**", tokens[1].Raw);
            Assert.Equal(TokenType.Number, tokens[2].Type);
            Assert.Equal("??", tokens[3].Raw);
            Assert.Equal(TokenType.PlusOperator, tokens[3].Type);
            Assert.Equal(TokenType.Number, tokens[4].Type);

            _tokenizerMock.Received(3).TryParseDelimiter(Arg.Is("1"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(1).TryParseDelimiter(Arg.Is("1*"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(1).TryParseDelimiter(Arg.Is("1**"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(1).TryParseDelimiter(Arg.Is("1?"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(1).TryParseDelimiter(Arg.Is("1??"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.DidNotReceive().TryParseDelimiter(Arg.Is((string s) => s.Length > 3 || !s.StartsWith("1")), out Arg.Any<string>(), out Arg.Any<string>());

            _tokenizerMock.Received(3).Identify(Arg.Is("1"));
            _tokenizerMock.Received(1).Identify(Arg.Is("**"));
            _tokenizerMock.Received(1).Identify(Arg.Is("??"));
            _tokenizerMock.DidNotReceive().Identify(Arg.Is((string s) => s != "1" && s != "**" && s != "??"));

        }

        [Fact]
        public void Should_Default_Delimitor_Work_When_Custom_Given()
        {
            _tokenizerMock.TryParseDelimiter(Arg.Any<string>(), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("1\\n"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(arg => { arg[1] = "\\n"; arg[2] = "1"; return true; });
            _tokenizerMock.Identify(Arg.Is("1")).Returns(new Token(TokenType.Number, "1", 1));
            _tokenizerMock.Identify(Arg.Is("\\n")).Returns(new Token(TokenType.PlusOperator, "\\n"));

            var actual = _lexer.Scan($"//[aaa]\\n1\\n1");

            Assert.Equal(3, actual.Count());
            var tokens = actual.ToArray();
            Assert.Equal(TokenType.Number, tokens[0].Type);
            Assert.Equal(TokenType.PlusOperator, tokens[1].Type);
            Assert.Equal("\\n", tokens[1].Raw);
            Assert.Equal(TokenType.Number, tokens[2].Type);

            _tokenizerMock.Received(1).TryParseDelimiter(Arg.Is("1\\n"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(2).TryParseDelimiter(Arg.Is("1"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.DidNotReceive().TryParseDelimiter(Arg.Is((string s) => s.Length > 4 || !s.StartsWith("1")), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(2).Identify(Arg.Is("1"));
            _tokenizerMock.Received(1).Identify(Arg.Is("\\n"));
            _tokenizerMock.DidNotReceive().Identify(Arg.Is((string s) => s != "1" && s != "\\n"));

        }

        [Fact]
        public void Should_Default_Delimitor_Work_When_Same_Custom_Given()
        {
            _tokenizerMock.TryParseDelimiter(Arg.Any<string>(), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("1,"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(arg => { arg[1] = ","; arg[2] = "1"; return true; });
            _tokenizerMock.Identify(Arg.Is("1")).Returns(new Token(TokenType.Number, "1", 1));
            _tokenizerMock.Identify(Arg.Is(",")).Returns(new Token(TokenType.PlusOperator, ","));

            var actual = _lexer.Scan($"//[,]\\n1,1");

            Assert.Equal(3, actual.Count());
            var tokens = actual.ToArray();
            Assert.Equal(TokenType.Number, tokens[0].Type);
            Assert.Equal(TokenType.PlusOperator, tokens[1].Type);
            Assert.Equal(",", tokens[1].Raw);
            Assert.Equal(TokenType.Number, tokens[2].Type);

            _tokenizerMock.Received(1).TryParseDelimiter(Arg.Is("1,"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(2).TryParseDelimiter(Arg.Is("1"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.DidNotReceive().TryParseDelimiter(Arg.Is((string s) => s.Length > 4 || !s.StartsWith("1")), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(2).Identify(Arg.Is("1"));
            _tokenizerMock.Received(1).Identify(Arg.Is(","));
            _tokenizerMock.DidNotReceive().Identify(Arg.Is((string s) => s != "1" && s != ","));

        }
        */
    }
}
