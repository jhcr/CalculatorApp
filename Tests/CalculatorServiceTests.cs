using CalculatorApp.ApplicationService;
using System;
using Xunit;

namespace Test
{
    public class CalculatorServiceTest
    {
        private CalculatorService _service;

        public CalculatorServiceTest()
        {
            _service = new CalculatorService();
        }

        [Theory]
        [InlineData("1,1", 2)]
        [InlineData("0,0", 0)]
        [InlineData("0, 0", 0)]
        void Should_Sum_When_Two_Numbers_Given(string input, int expected)
        {
            var result = _service.Run(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(",1", 1)]
        [InlineData(" ,1", 1)]
        [InlineData("cdscs,1", 1)]
        [InlineData(",", 0)]
        [InlineData("cdscs,fdsfs rgre", 0)]
        void Should_Sum_When_Invalid_Number_Given(string input, int expected)
        {
            var result = _service.Run(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1,1,1", 3)]
        [InlineData("0,0,1,1,1,1", 4)]
        [InlineData("cdscs, 1 ,fd%sfs rgre, ,,1,1", 3)]
        void Should_Sum_When_More_Than_Two_Numbers_Given(string input, int expected)
        {
            var result = _service.Run(input);
            Assert.Equal(expected, result);
        }
    }
}
