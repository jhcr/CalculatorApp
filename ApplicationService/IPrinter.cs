using CalculatorApp.EntityService;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculatorApp.ApplicationService
{
    public interface IPrinter
    {
        void Print(IEnumerable<Token> token, int result);
        void Print(Token token);
        void Print(int result);
    }
}
