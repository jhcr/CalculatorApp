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
        ICustomizer _customizerMock;

        public Lexer_UnitTests()
        {
            _tokenizerMock = Substitute.For<ITokenizer>();
            _customizerMock = Substitute.For<ICustomizer>();
            _lexer = new Lexer(_tokenizerMock, _customizerMock);
        }

        [Fact]
        public void Should_Identify_All_Delimiters_When_Multiple_Delimiters_Given()
        {
            _tokenizerMock.TryParseDelimiter(Arg.Is("1"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("1,"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(arg => { arg[1] = ","; arg[2] = "1"; return true; });
            _tokenizerMock.Identify(Arg.Is("1")).Returns(new Token(TokenType.Number,"1",1));
            _tokenizerMock.Identify(Arg.Is(",")).Returns(new Token(TokenType.PlusOperator, ","));
            _customizerMock.Config(Arg.Any<ITokenizer>(), Arg.Any<string>()).Returns("1,1,1");

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
            _customizerMock.Config(Arg.Any<ITokenizer>(), Arg.Any<string>()).Returns("1,1,");

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
            _customizerMock.Config(Arg.Any<ITokenizer>(), Arg.Any<string>()).Returns("1,-1,-1");
            _customizerMock.DenyNegativeNumbers.Returns(true);

            Assert.Throws<NagativeNumberException>(() => _lexer.Scan("1,-1,-1"));

            _tokenizerMock.Received(1).TryParseDelimiter(Arg.Is("1,"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(1).TryParseDelimiter(Arg.Is("-1,"), out Arg.Any<string>(), out Arg.Any<string>());
            _tokenizerMock.Received(2).Identify(Arg.Is("-1"));

        }

        [Fact]
        public void Should_Not_Throw_NagativeNumberException_When_Deny_Negative_Number_Disabled_Given()
        {
            _tokenizerMock.TryParseDelimiter(Arg.Is("1"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("1,"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(arg => { arg[1] = ","; arg[2] = "1"; return true; });
            _tokenizerMock.TryParseDelimiter(Arg.Is("-"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("-1"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(false);
            _tokenizerMock.TryParseDelimiter(Arg.Is("-1,"), out Arg.Any<string>(), out Arg.Any<string>()).Returns(arg => { arg[1] = ","; arg[2] = "-1"; return true; });
            _tokenizerMock.Identify(Arg.Is("1")).Returns(new Token(TokenType.Number, "1", 1));
            _tokenizerMock.Identify(Arg.Is(",")).Returns(new Token(TokenType.PlusOperator, ","));
            _tokenizerMock.Identify(Arg.Is("-1")).Returns(new Token(TokenType.Number, "-1", -1));
            _customizerMock.Config(Arg.Any<ITokenizer>(), Arg.Any<string>()).Returns("1,-1,-1");
            _customizerMock.DenyNegativeNumbers.Returns(false);

            _lexer.Scan("1,-1,-1");

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

    }
}
