using CalculatorApp.EntityService;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests
{
    public class Tokenizor_UnitTests
    {
        private Tokenizor _tokenizor;

        public Tokenizor_UnitTests()
        {
            _tokenizor = new Tokenizor();
        }

        [Theory]
        [InlineData("1,", ",", "1", true)]
        [InlineData("1\\n", "\\n","1", true)]
        [InlineData("1,1,", ",", "1,1", true)]
        void Should_IsEndByDelimiter_Work_When_Default_Delimiter_Used(string input, string expectedDelimiter, string expectedLiteral, bool expected)
        {
            var actual = _tokenizor.TryParseDelimiter(input, out var actualDelimiter, out var actualLiteral);
            Assert.Equal(expectedDelimiter, actualDelimiter);
            Assert.Equal(expectedLiteral, actualLiteral);
            Assert.Equal(expected, actual);

        }

        [Fact]
        void Should_Identify_When_Valid_Lex_Given()
        {
            var lex = "35";
            var actual = _tokenizor.Identify(lex);

            Assert.Equal(lex, actual.Raw);
            Assert.Equal(int.Parse(lex), actual.Value);
            Assert.Equal(TokenType.Number, actual.Type);

            lex = ",";
            actual = _tokenizor.Identify(lex);

            Assert.Equal(lex, actual.Raw);
            Assert.Null(actual.Value);
            Assert.Equal(TokenType.PlusOperator, actual.Type);

            lex = "\\n";
            actual = _tokenizor.Identify(lex);

            Assert.Equal(lex, actual.Raw);
            Assert.Null(actual.Value);
            Assert.Equal(TokenType.PlusOperator, actual.Type);
        }

        [Fact]
        void Should_Convert_To_Zero_When_Invalid_Lex_Given()
        {
            var lex = "aaa";
            var actual = _tokenizor.Identify(lex);

            Assert.Equal(lex, actual.Raw);
            Assert.Equal(0, actual.Value);
            Assert.Equal(TokenType.Number, actual.Type);
        }

        [Fact]
        void Should_Identify_As_IgnoredNumber_When_Number_Is_Larger_Than_1000()
        {
            var lex = "1001";
            var actual = _tokenizor.Identify(lex);

            Assert.Equal(lex, actual.Raw);
            Assert.Null(actual.Value);
            Assert.Equal(TokenType.IgnoredNumber, actual.Type);
        }

        [Fact]
        void Should_Identify_As_Number_When_Number_Equals_1000()
        {
            var lex = "1000";
            var actual = _tokenizor.Identify(lex);

            Assert.Equal(lex, actual.Raw);
            Assert.Equal(int.Parse(lex), actual.Value);
            Assert.Equal(TokenType.Number, actual.Type);
        }

        [Fact]
        void Should_Throw_OverlappingDelimitersException_When_Try_Add_Overlapping_Delimiters()
        {
            Assert.Throws<OverlappingDelimitersException>(() =>
            {
                _tokenizor.ApplyConfig("ab", TokenType.PlusOperator);
                _tokenizor.ApplyConfig("abc", TokenType.PlusOperator);
            }
            );
        }
    }
}
