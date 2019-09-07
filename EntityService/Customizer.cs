using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CalculatorApp.EntityService
{
    /// <summary>
    /// Customize for Tokenizer
    /// </summary>
    public class Customizer: ICustomizer
    {
        /// <summary>
        /// Parse custom delimiters from text and apply to lexer, if no custom one, apply the default
        /// </summary>
        /// <param name="target">lexer</param>
        /// <param name="text">source text contain custom delimiters, config secction will be removed if found</param>
        public void Config(ILexer target, ref string text)
        {
            if (TryParse(text, out var offset, out Dictionary<string, TokenType> delimiters))
            {
                target.ApplyConfig(delimiters);
                text = text.Remove(0, offset);
            }
            else
            {
                target.ApplyDefaultConfig();
            }
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
