using CalculatorApp.EntityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CalculatorApp.ApplicationService
{
    public class CalculatorService
    {
        private ICustomizer _customizer;
        private ILexer _lexer;

        public CalculatorService(ICustomizer customizer, ILexer lexer)
        {
            _customizer = customizer;
            _lexer = lexer;
        }

        public int Run(string input)
        {
            _customizer.Config(_lexer, ref input);

            var tokens = _lexer.Scan(input);

            return tokens?.Where(t => t.Type == TokenType.Number)?.Select(t => t.Value)?.Sum() ?? 0;


        }

       
    }
}
