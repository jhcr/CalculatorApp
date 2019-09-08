using CalculatorApp.ApplicationService;
using System;
using Microsoft.Extensions.DependencyInjection;
using CalculatorApp.EntityService;
using CalculatorApp.Infrastructure;

namespace CaculatorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var running = true;

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                running = false;
                e.Cancel = false;
            };

            var serviceProvider = ConfigureServices();

            var service = serviceProvider.GetService<CalculatorService>();
            var config = serviceProvider.GetService<IConfiguration>();

            config.ParseFrom(args);

            while (running)
            {
                Console.WriteLine("Input>");

                var input = Console.ReadLine();

                if (input == null)
                    break;

                try
                {
                    var result = service.Run(input);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }

        static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<IDelimiterParser, DelimiterParser>()
                .AddSingleton<ITokenizer, Tokenizor>()
                .AddSingleton<IPrinter, Printer>()
                .AddSingleton<ILexer, Lexer>()
                .AddSingleton<CalculatorService>()
                .BuildServiceProvider();
        }
    }
}
