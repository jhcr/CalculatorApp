using System;
using System.Collections.Generic;
using System.Text;

namespace CalculatorApp.EntityService
{
    public class OverlappingDelimitersException : Exception
    {
        public IEnumerable<string> Delimiters { get; set; }

        public OverlappingDelimitersException(IEnumerable<string> delimiters)
            :base($"Overlapping Delimiters denied: {string.Join(", ", delimiters)}")
        {
            Delimiters = delimiters;
        }
    }
}
