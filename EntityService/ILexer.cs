using System.Collections.Generic;

namespace CalculatorApp.EntityService
{
    public interface ILexer
    {
        IEnumerable<Token> Scan(string source);
        void ApplyDefaultConfig();
        void ApplyConfig(IDictionary<string, TokenType> delimiters);
    }
}