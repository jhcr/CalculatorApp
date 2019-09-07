using System;
using System.Collections.Generic;
using System.Text;

namespace CalculatorApp.EntityService
{
    public class OverlappingDelimitorsException : Exception
    {
        public IEnumerable<string> Delimitors { get; set; }

        public OverlappingDelimitorsException(IEnumerable<string> delimitors)
            :base($"Overlapping Delimitors denied: {string.Join(", ", delimitors)}")
        {
            Delimitors = delimitors;
        }
    }
}
