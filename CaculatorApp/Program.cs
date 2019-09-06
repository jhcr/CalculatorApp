using CalculatorApp.ApplicationService;
using System;
using Microsoft.Extensions.DependencyInjection;
using CalculatorApp.EntityService;

namespace CaculatorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = SetupCaculatorApp();

            var tokenizer = serviceProvider.GetService<ITokenizer>();

            Console.WriteLine("Input>");

            var input = Console.ReadLine();

            var service = new CalculatorService(tokenizer);

            var result = service.Run(input);

            Console.WriteLine(result);

            Console.ReadKey();
        }

        static ServiceProvider SetupCaculatorApp()
        {
            return new ServiceCollection()
                .AddSingleton<ITokenizer, Tokenizor>()
                .BuildServiceProvider();
        }
    }
}
