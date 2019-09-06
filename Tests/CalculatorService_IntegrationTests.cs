using CalculatorApp.ApplicationService;
using CalculatorApp.EntityService;
using System;
using Xunit;

namespace Test
{
    public class CalculatorService_IntegrationTests
    {
        private CalculatorService _service;

        public CalculatorService_IntegrationTests()
        {
            _service = new CalculatorService(new Tokenizor());
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
    }
}
