using CalculatorApp.ApplicationService;
using CalculatorApp.EntityService;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculatorApp.Infrastructure
{
    public class Printer: IPrinter
    {
        public void Print(IEnumerable<Token> tokens, int result)
        {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            var enu = tokens.GetEnumerator();
            while (enu.MoveNext())
            {
                Print(enu.Current);
            }
            Print(result);
        }

        public void Print(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                    Console.Write(token.Value);
                    break;
                case TokenType.IgnoredNumber:
                    Console.Write(token.Value?? 0);
                    break;
                case TokenType.PlusOperator:
                    Console.Write("+");
                    break;
                case TokenType.MinusOperator:
                    Console.Write("-");
                    break;
                case TokenType.MultiplyOperator:
                    Console.Write("*");
                    break;
                case TokenType.DivideOperator:
                    Console.Write("/");
                    break;
                default:
                    throw new NotSupportedException($"Unregonized token: {token.Raw}");
            }
        }

        public void Print(int result)
        {
            Console.WriteLine($" = {result}");
        }        
    }
}
