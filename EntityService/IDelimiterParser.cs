using System.Collections.Generic;

namespace CalculatorApp.EntityService
{
    public interface IDelimiterParser
    {
        bool TryParse(string text, out int offset, out Dictionary<string, TokenType> delimiters);
    }
}