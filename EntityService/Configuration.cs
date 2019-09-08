using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CalculatorApp.EntityService
{
    public class Configuration: IConfiguration
    {
        public string AlternateDelimiter { get; set; }
        public bool DenyNegativeNumbers { get; set; }
        public int NumberUpperBound { get; set; }
        public Dictionary<string, TokenType> DefaultRule { get; set; }

        public Configuration()
        {
            AlternateDelimiter = "\\n";
            DenyNegativeNumbers = true;
            NumberUpperBound = 1000;
            DefaultRule = new Dictionary<string, TokenType>() {
                { ",", TokenType.PlusOperator } };
        }

        /// <summary>
        /// Parse commandline arguments into global settings
        /// </summary>
        /// <param name="args"></param>
        public void ParseFrom(string[] args)
        {
            for (var x = 0; x < args.Length - 1; x++)
            {
                switch (args[x].Trim().ToUpper())
                {
                    case "/A":
                        if (!Regex.IsMatch(args[x + 1], "/[ADU]"))
                            AlternateDelimiter = args[x + 1];
                        break;
                    case "/D":
                        if (bool.TryParse(args[x + 1], out var deny))
                            DenyNegativeNumbers = deny;
                        else if (args[x + 1] == "0")
                            DenyNegativeNumbers = false;
                        else if (args[x + 1] == "1")
                            DenyNegativeNumbers = true;
                        break;
                    case "/U":
                        if (int.TryParse(args[x + 1], out var num))
                            NumberUpperBound = num;
                        break;
                }
            }
        }
    }
}
