using CalculatorApp.EntityService;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using NSubstitute;

namespace Tests
{
    public class DelimiterParser_UnitTests
    {
        private IDelimiterParser _parser;

        public DelimiterParser_UnitTests()
        {
            _parser = new DelimiterParser();
        }

        [Fact]
        void Should_Throw_OverlappingDelimitersException_When_Try_Add_Overlapping_Delimiters()
        {
            Assert.Throws<OverlappingDelimitersException>(() =>
            {
                _parser.TryParse("//[a][aa]\\n1a1", out var offset, out Dictionary<string, TokenType> delimiters);
            });
        }

        [Fact]
        void Should_Parse_When_Single_Char_Delimiter_Given()
        {
            var actual = _parser.TryParse("//a\\n1a1", out var offset, out Dictionary<string, TokenType> delimiters);

            Assert.True(actual);
            Assert.True(delimiters.Count == 1 && delimiters.ContainsKey("a"));
            Assert.Equal(5, offset);
        }

        [Fact]
        void Should_Parse_When_Any_Length_Delimiter_Given()
        {
            var actual = _parser.TryParse("//[aa][bbb][ccc]\\n1a1", out var offset, out Dictionary<string, TokenType> delimiters);

            Assert.True(actual);
            Assert.True(delimiters.Count == 3 && delimiters.ContainsKey("aa") && delimiters.ContainsKey("bbb")&& delimiters.ContainsKey("ccc"));
            Assert.Equal(18, offset);
        }

        [Fact]
        void Should_Not_Parse_When_Reserved_Keyword_Given()
        {
            var actual = _parser.TryParse("//[a[]\\n1a1", out var offset, out Dictionary<string, TokenType> delimiters);

            Assert.False(actual);
        }

        [Fact]
        void Should_Not_Parse_When_No_Custom_Delimiter_Given()
        {
            var actual = _parser.TryParse("\\n1a1", out var offset, out Dictionary<string, TokenType> delimiters);

            Assert.False(actual);
        }

        [Fact]
        void Should_Not_Parse_When_No_Empty_Delimiter_Given()
        {
            var actual = _parser.TryParse("//\\n1a1", out var offset, out Dictionary<string, TokenType> delimiters);

            Assert.False(actual);
        }

    }
}
