using System;
using System.Collections.Generic;
using System.Text;

namespace CalculatorApp.EntityService
{
    public class NagativeNumberException: Exception
    {
        public IEnumerable<int> Numbers { get; set; }

        public NagativeNumberException(IEnumerable<int> numbers)
            :base($"Negative number(s) denied: {string.Join(", ", numbers)}")
        {
            Numbers = numbers;
        }
    }
}
