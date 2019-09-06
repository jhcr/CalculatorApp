using CalculatorApp.EntityService;
using System;
using System.Linq;

namespace CalculatorApp.ApplicationService
{
    public class CalculatorService
    {
        private ITokenizer _tokenizor;

        public CalculatorService(ITokenizer tokenizor)
        {
            _tokenizor = tokenizor;
        }

        public int Run(string input)
        {/*
            var lexemes = input?.Split(',');
            var sum = 0;
            for (int i=0; i <lexemes.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lexemes[i]) && int.TryParse(lexemes[i].Trim(), out var number))
                    sum += number;
            }

            return (_previousSum += sum);
            */
            var lexer = new Lexer(_tokenizor);
            var tokens = lexer.Scan(input);


            return tokens?.Where(t=>t.Type == TokenType.Number)?.Select(t=>t.Value)?.Sum()??0;

        }
    }
}
