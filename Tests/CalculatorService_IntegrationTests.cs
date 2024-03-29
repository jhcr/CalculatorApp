using CalculatorApp.ApplicationService;
using CalculatorApp.EntityService;
using CalculatorApp.Infrastructure;
using System;
using Xunit;

namespace Tests
{
    public class CalculatorService_IntegrationTests
    {
        private CalculatorService _service;
        private IConfiguration _config;

        public CalculatorService_IntegrationTests()
        {
            _config = new Configuration();
            _service = new CalculatorService(new Lexer(new Tokenizor(_config), new DelimiterParser(), _config), new Printer());
        }

        [Theory]
        [InlineData("1", 1)]
        void Should_Sum_When_One_Number_Given(string input, int expected)
        {
            var actual = _service.Run(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("1,2", 3)]
        [InlineData("0,0", 0)]
        [InlineData("0, 0", 0)]
        void Should_Sum_When_Two_Numbers_Given(string input, int expected)
        {
            var actual = _service.Run(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(",1", 1)]
        [InlineData(" ,1", 1)]
        [InlineData("aaa,1", 1)]
        [InlineData(",", 0)]
        [InlineData("1,", 1)]
        [InlineData("aaa,bbb ccc", 0)]
        [InlineData("", 0)]
        void Should_Convert_Invalid_Number_To_Zero_When_Invalid_Number_Given(string input, int expected)
        {
            var actual = _service.Run(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("1,0,2", 3)]
        [InlineData("aaa, 1 ,bb%bb ccc, ,,1,2", 4)]
        void Should_Sum_When_More_Than_Two_Numbers_Given(string input, int expected)
        {
            var actual = _service.Run(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("1\\n1,1", 3)]
        [InlineData("\\n1,1", 2)]
        [InlineData("1\\n", 1)]
        [InlineData("1\\n1,1\\n1,1", 5)]
        [InlineData("aaa\\n", 0)]
        [InlineData("aaa \\n\\n,1,1", 2)]
        void Should_Sum_When_NewLine_Character_Given(string input, int expected)
        {
            var actual = _service.Run(input);
            Assert.Equal(expected, actual);
        }

        [Fact]
        void Should_Throw_NagativeNumberException_When_Negative_Number_Given()
        {
            var input = "1,-1";
            Assert.Throws<NagativeNumberException>(() => _service.Run(input));
        }

        [Fact]
        void Should_Sum_Ignore_Number_More_Than_1000()
        {
            var actual = _service.Run("1001,1002,1");
            Assert.Equal(1, actual);
        }

        [Fact]
        void Should_Sum_Include_Number_1000()
        {
            var actual = _service.Run("1000,1");
            Assert.Equal(1001, actual);
        }

        [Theory]
        [InlineData("//a\\n1a1", 2)]
        void Should_Sum_When_Single_Custom_Delimiter_Given(string input, int expected)
        {
            var actual = _service.Run(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("//aa\\n1a1", 0)]
        void Should_Treat_As_Invalid_When_Single_Custom_Delimiter_Has_Invalid_Format(string input, int expected)
        {
            var actual = _service.Run(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("//[a]\\n1a1", 2)]
        [InlineData("//[a1][b2][c3]\\n1a12b23c34", 10)]
        [InlineData("//[a1][b2][c3]\\n1 a1 2b23c3", 6)]
        [InlineData("//[*][!!][XXX]\\n1*2!!XXX3", 6)]
        void Should_Sum_When_Multiple_Custom_Delimiters_Given(string input, int expected)
        {
            var actual = _service.Run(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("//[a][a]\\n1a1", 2)]
        void Should_Sum_When_Duplicate_Custom_Delimiters_Given(string input, int expected)
        {
            var actual = _service.Run(input);
            Assert.Equal(expected, actual);
        }

        [Fact]
        void Should_Throw_OverlappingDelimitersException_When_Try_Add_Overlapping_Delimiters()
        {
            Assert.Throws<OverlappingDelimitersException>(() => _service.Run("//[ab][abc][abcd]\\n1abc1abcd1"));
        }

        [Fact]
        void Should_Not_Throw_OverlappingDelimitersException_When_Default_Delimier_Given()
        {
            var actual = _service.Run("//[,]\\n1,1");
            Assert.Equal(2, actual);
        }

        [Theory]
        [InlineData("//+[+]-[!]*[*]/[$$]\\n2*4", 8)]
        [InlineData("//+[+]-[!]*[*]/[$$]\\n2*4*6", 48)]
        [InlineData("//+[+]-[!]*[*]/[$$]\\n1+2*4!2", 7)]
        [InlineData("//+[+]-[!]*[*]/[$$]\\n1+2*4!2$$2", 8)]
        [InlineData("//+[+]-[-]*[*]/[/]\\n2*5+6/3*4-2+6/3*2", 20)]
        void Should_Calculate_When_Mixed_Operators_Given(string input, int expected)
        {
            var actual = _service.Run(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("//+[+]-[-]*[*]/[/]\\n1+2000", 1)]
        [InlineData("//+[+]-[-]*[*]/[/]\\n1*2000", 1)]
        [InlineData("//+[+]-[-]*[*]/[/]\\n1+2000-3*4000", -2)]
        void Should_Ignored_Correctly_When_Large_Numbers_Given(string input, int expected)
        {
            var actual = _service.Run(input);
            Assert.Equal(expected, actual);
        }
    }
}
