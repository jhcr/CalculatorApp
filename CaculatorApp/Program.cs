using CalculatorApp.ApplicationService;
using System;

namespace CaculatorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = Console.ReadLine();
            var service = new CalculatorService();
            var result = service.Run(input);
            Console.WriteLine(result);
        }
    }
}
