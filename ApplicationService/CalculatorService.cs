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
        private IPrinter _printer;

        public CalculatorService(ICustomizer customizer, ILexer lexer, IPrinter printer)
        {
            _customizer = customizer;
            _lexer = lexer;
            _printer = printer;
        }

        public int Run(string input)
        {
            _customizer.Config(_lexer, ref input);

            var tokens = _lexer.Scan(input);

            var result = tokens?.Where(t => t.Type == TokenType.Number)?.Select(t => t.Value)?.Sum() ?? 0;

            _printer.Print(tokens, result);

            return result;

        }

       
    }
}
