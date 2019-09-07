﻿using CalculatorApp.EntityService;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using NSubstitute;

namespace Tests
{
    public class Customizer_UnitTests
    {
        private Customizer _customizer;
        private ITokenizer _tokenizor;

        public Customizer_UnitTests()
        {
            _customizer = new Customizer();
            _tokenizor = Substitute.For<ITokenizer>();
        }

        [Fact]
        void Should_Tokonizer_Receive_Delimitor_When_Single_Custom_Delimiter_Given()
        {
            var actual = _customizer.Config(_tokenizor, "//a\\n1a1");

            Assert.Equal("1a1", actual);
            _tokenizor.ReceivedWithAnyArgs(1).ApplyDefaultConfig();
            _tokenizor.ReceivedWithAnyArgs(1).ApplyConfig(Arg.Is<IDictionary<string, TokenType>>(o=>o.ContainsKey("a") && o.Keys.Count == 1));
        }

        [Fact]
        void Should_Tokonizer_Receive_Delimitor_When_Any_Length_Custom_Delimiter_Given()
        {
            var actual = _customizer.Config(_tokenizor, "//[aa][bbb][ccc]\\n1a1");

            Assert.Equal("1a1", actual);
            _tokenizor.ReceivedWithAnyArgs(1).ApplyDefaultConfig();
            _tokenizor.ReceivedWithAnyArgs(1).ApplyConfig(Arg.Is<IDictionary<string, TokenType>>(o => o.ContainsKey("aa") 
            && o.ContainsKey("bbb") && o.ContainsKey("ccc") && o.Keys.Count == 3));
        }

        [Fact]
        void Should_Reserve_Bracket_When_Any_Length_Custom_Delimiter_Given()
        {
            var actual = _customizer.Config(_tokenizor, "//[a[]\\n1a1");

            Assert.Equal("//[a[]\\n1a1", actual);
            _tokenizor.ReceivedWithAnyArgs(1).ApplyDefaultConfig();
            _tokenizor.ReceivedWithAnyArgs(0).ApplyConfig(Arg.Any<IDictionary<string, TokenType>>());
        }

        [Fact]
        void Should_Read_Args_When_All_Given()
        {
            _customizer.ReadArguments(new string[] { "/D","1","/A","\\r", "/U", "200"});

            Assert.True(_customizer.DenyNegativeNumbers);
            Assert.Equal("\\r", _customizer.GlobalAlternateDelimiter);
            Assert.Equal(200, _customizer.NumberUpperBound);
        }

        [Fact]
        void Should_Read_Args_When_Some_Given()
        {
            _customizer.ReadArguments(new string[] { "/D", "1", "/A", "\\r" });

            Assert.True(_customizer.DenyNegativeNumbers);
            Assert.Equal("\\r", _customizer.GlobalAlternateDelimiter);
            Assert.Equal(1000, _customizer.NumberUpperBound);
        }

        [Fact]
        void Should_Not_Set_When_Reserve_Keyword_Given()
        {
            _customizer.ReadArguments(new string[] { "/A", "/U", "200" });

            Assert.Equal("\\n", _customizer.GlobalAlternateDelimiter);
            Assert.Equal(200, _customizer.NumberUpperBound);
        }

    }
}