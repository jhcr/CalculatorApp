using CalculatorApp.EntityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CalculatorApp.ApplicationService
{
    public class CalculatorService
    {
        private ILexer _lexer;
        private IPrinter _printer;

        public CalculatorService(ILexer lexer, IPrinter printer)
        {
            _lexer = lexer;
            _printer = printer;
        }

        public int Run(string input)
        {
            var tokens = _lexer.Scan(input);

            var result = tokens?.Where(t => t.Type == TokenType.Number)?.Select(t => t.Value)?.Sum() ?? 0;

            _printer.Print(tokens, result);

            return result;

        }

       
    }
}
