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
            var running = true;

            var serviceProvider = SetupCaculatorApp();

            var service = serviceProvider.GetService<CalculatorService>();

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                running = false;
            };

            while (running)
            {
                Console.WriteLine("Input>");

                var input = Console.ReadLine();

                try
                {
                    var result = service.Run(input);

                    Console.WriteLine(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }

        static ServiceProvider SetupCaculatorApp()
        {
            return new ServiceCollection()
                .AddSingleton<ICustomizer, Customizer>()
                .AddSingleton<ITokenizer, Tokenizor>()
                .AddSingleton<ILexer, Lexer>()
                .AddSingleton<CalculatorService>()
                .BuildServiceProvider();
        }
    }
}
