using System;
using System.Collections.Generic;
using System.Text;

namespace CalculatorApp.EntityService
{
    public class InvalidSyntaxException : Exception
    {
        public Token Token { get; set; }
        public InvalidSyntaxException(Token token)
            : base($"Syntax error at \"{token.Raw}\"")
        {
            Token = token;
        }
    }
}
