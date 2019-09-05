using System;

namespace CalculatorApp.ApplicationService
{
    public class CalculatorService
    {
        public int Run(string input)
        {
            var lexemes = input?.Split(',');
            var sum = 0;
            for (int i=0; i <lexemes.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lexemes[i]) && int.TryParse(lexemes[i].Trim(), out var number))
                    sum += number;
            }

            return sum;
        }
    }
}
