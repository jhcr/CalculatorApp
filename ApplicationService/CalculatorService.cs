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

            var ast = new AST(tokens);

            ast.EvaluationCallBack = (token) => { _printer.Print(token); };

            var result = ast.Evaluate() ?? 0;

            _printer.Print(result);

            //_printer.Print(tokens, result);

            return result;

        }

       
    }
}
