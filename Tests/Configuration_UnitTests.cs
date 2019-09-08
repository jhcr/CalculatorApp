using CalculatorApp.EntityService;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using NSubstitute;

namespace Tests
{
    public class Configuration_UnitTests
    {
        public Configuration_UnitTests()
        {
        }

        [Fact]
        void Should_Default_Value_When_Init_Given()
        {
            var config = new Configuration();

            Assert.True(config.DenyNegativeNumbers);
            Assert.Equal("\\n", config.AlternateDelimiter);
            Assert.Equal(1000, config.NumberUpperBound);
            Assert.True(config.DefaultRule!= null && config.DefaultRule.ContainsKey(","));
        }


        [Fact]
        void Should_ParseFrom_When_All_Args_Given()
        {
            var config = new Configuration();
            config.ParseFrom(new string[] { "/D","1","/A","\\r", "/U", "200"});

            Assert.True(config.DenyNegativeNumbers);
            Assert.Equal("\\r", config.AlternateDelimiter);
            Assert.Equal(200, config.NumberUpperBound);
        }

        [Fact]
        void Should_ParseFrom_When_Partial_Args_Given()
        {
            var config = new Configuration();
            config.ParseFrom(new string[] { "/D", "1", "/A", "\\r" });

            Assert.True(config.DenyNegativeNumbers);
            Assert.Equal("\\r", config.AlternateDelimiter);
            Assert.Equal(1000, config.NumberUpperBound);
        }

        [Fact]
        void Should_Not_ParseFrom_When_Reserve_Keyword_Given()
        {
            var config = new Configuration();
            config.ParseFrom(new string[] { "/A", "/U", "200" });

            Assert.Equal("\\n", config.AlternateDelimiter);
            Assert.Equal(200, config.NumberUpperBound);
        }
    }
}
