using System;
using System.Collections.Generic;
using System.Text;

namespace CalculatorApp.EntityService
{
    public interface ITokenizer
    {
        Token Identify(string lex);
        bool TryParseDelimiter(string lex, out string delimiter, out string literal);
        void ApplyCustomRule(IDictionary<string, TokenType> delimiters);
        void ResetCustomRule();
    }

}
