using CalculatorApp.EntityService;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests
{
    public class AST_UnitTests
    {
        public AST_UnitTests()
        {
        }

        [Fact]
        void Should_Evaluate_When_PlusOperator_Given()
        {
            var tokens = new List<Token>() {
                new Token(TokenType.Number,"1",1),
                new Token(TokenType.PlusOperator,"+"),
                new Token(TokenType.Number,"2",2),
            };
            var ast = new AST(tokens);
            var actual = ast.Evaluate();

            Assert.Equal(3, actual);
        }

        [Fact]
        void Should_Evaluate_When_MinusOperator_Given()
        {
            var tokens = new List<Token>() {
                new Token(TokenType.Number,"3",3),
                new Token(TokenType.MinusOperator,"-"),
                new Token(TokenType.Number,"2",2),
            };
            var ast = new AST(tokens);
            var actual = ast.Evaluate();

            Assert.Equal(1, actual);
        }

        [Fact]
        void Should_Evaluate_When_MultiplyOperator_Given()
        {
            var tokens = new List<Token>() {
                new Token(TokenType.Number,"3",3),
                new Token(TokenType.MultiplyOperator,"*"),
                new Token(TokenType.Number,"2",2),
            };
            var ast = new AST(tokens);
            var actual = ast.Evaluate();

            Assert.Equal(6, actual);
        }

        [Fact]
        void Should_Evaluate_When_DivideOperator_Given()
        {
            var tokens = new List<Token>() {
                new Token(TokenType.Number,"6",6),
                new Token(TokenType.DivideOperator,"/"),
                new Token(TokenType.Number,"2",2),
            };
            var ast = new AST(tokens);
            var actual = ast.Evaluate();

            Assert.Equal(3, actual);
        }

        [Fact]
        void Should_Evaluate_When_Multiple_Operators_Given()
        {
            var tokens = new List<Token>() {
                new Token(TokenType.Number,"1",1),
                new Token(TokenType.MinusOperator,"-"),
                new Token(TokenType.Number,"2",2),
                new Token(TokenType.MultiplyOperator,"*"),
                new Token(TokenType.Number,"3",3),
                new Token(TokenType.PlusOperator,"+"),
                new Token(TokenType.Number,"10",10),
                 new Token(TokenType.DivideOperator,"/"),
                new Token(TokenType.Number,"5",5),
            };
            var ast = new AST(tokens);
            var actual = ast.Evaluate();

            Assert.Equal(-3, actual);
        }

        [Fact]
        void Should_Throw_InvalidSyntaxException_When_Invalid_Token_Given()
        {
            var tokens = new List<Token>() {
                new Token(TokenType.PlusOperator,","),
                new Token(TokenType.Number,"2",2),
            };

            Assert.Throws<InvalidSyntaxException>(()=>new AST(tokens));

        }
    }
}
