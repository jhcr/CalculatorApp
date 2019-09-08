using System.Collections.Generic;

namespace CalculatorApp.EntityService
{
    public interface IConfiguration
    {
        string AlternateDelimiter { get; set; }
        bool DenyNegativeNumbers { get; set; }
        int NumberUpperBound { get; set; }
        Dictionary<string, TokenType> DefaultRule { get; set; }

        void ParseFrom(string[] args);
    }
}