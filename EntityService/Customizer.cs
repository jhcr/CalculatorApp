using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CalculatorApp.EntityService
{
    /// <summary>
    /// Customize for Lexer
    /// </summary>
    public class Customizer: ICustomizer
    {
        public string GlobalAlternateDelimiter { get; set; }
        public bool DenyNegativeNumbers { get; set; }
        public int NumberUpperBound { get; set; }

        public Customizer()
        {
            GlobalAlternateDelimiter = "\\n" ;
            DenyNegativeNumbers = true;
            NumberUpperBound = 1000;
        }

        /// <summary>
        /// Parse commandline arguments into global settings
        /// </summary>
        /// <param name="args"></param>
        public void ReadArguments(string[] args)
        {
            for (var x = 0; x < args.Length - 1; x++)
            {
                switch (args[x].Trim().ToUpper())
                {
                    case "/A":
                        if (!Regex.IsMatch(args[x + 1],"/[ADU]"))
                            GlobalAlternateDelimiter = args[x + 1];
                        break;
                    case "/D":
                        if (bool.TryParse(args[x + 1], out var deny))
                            DenyNegativeNumbers = deny;
                        break;
                    case "/U":
                        if( int.TryParse(args[x + 1], out var num))
                            NumberUpperBound = num;
                        break;
                }
            }
        }

        /// <summary>
        /// Parse custom delimiters from text and apply to tokenizer, if no custom one, apply the default
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="text">source text contain custom delimiters</param>
        /// <returns>the text without custom section</returns>
        public string Config(ITokenizer target, string text)
        {
            target.ApplyDefaultConfig(new Dictionary<string, TokenType>() {
                { ",", TokenType.PlusOperator },
                { GlobalAlternateDelimiter, TokenType.PlusOperator } });

            if (TryParse(text, out var offset, out Dictionary<string, TokenType> delimiters))
            {
                target.ApplyConfig(delimiters);
                text = text.Remove(0, offset);
            }
            return text;
        }

        /// <summary>
        /// Parse custom delimiters from text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="offset">starting position of the actual content</param>
        /// <param name="delimiters">delimiters</param>
        /// <returns></returns>
        private bool TryParse(string text, out int offset, out Dictionary<string, TokenType> delimiters)
        {
            delimiters = default(Dictionary<string, TokenType>);
            offset = 0;

            var c = new Regex($"(?<custom>^//(?<delimiter>.)\\\\n).*").Matches(text);
            if (c.Count == 1 && c[0].Success)
            {
                delimiters = new Dictionary<string, TokenType> { { c[0].Groups["delimiter"].Value, TokenType.PlusOperator } };
                offset = c[0].Groups["custom"].Length;
                return true;

            }

            c = new Regex($"(?<custom>^//(?<item>\\[(?<delimiter>[^\\[\\]]*)\\])+\\\\n).*").Matches(text);
            if (c.Count == 1 && c[0].Success)
            {
                var enu = c[0].Groups["delimiter"].Captures.GetEnumerator();
                delimiters = new Dictionary<string, TokenType>();
                while (enu.MoveNext())
                {
                    delimiters.Add((enu.Current as Capture).Value, TokenType.PlusOperator);
                }
                offset = c[0].Groups["custom"].Length;
                return true;

            }

            return false;
        }
    }
}
