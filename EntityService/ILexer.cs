using System.Collections.Generic;

namespace CalculatorApp.EntityService
{
    public interface ILexer
    {
        IEnumerable<Token> Scan(string source);
    }
}