using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace CalculatorApp.EntityService
{
    /// <summary>
    /// Customize for Lexer
    /// </summary>
    public class DelimiterParser: IDelimiterParser
    {
        /// <summary>
        /// Parse custom delimiters from text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="offset">starting position of the actual content</param>
        /// <param name="delimiters">delimiters</param>
        /// <returns></returns>
        public bool TryParse(string text, out int offset, out Dictionary<string, TokenType> delimiters)
        {
            delimiters = default(Dictionary<string, TokenType>);
            offset = 0;

            // for delimiter of single char
            var c = new Regex("(?<custom>^//(?<delimiter>.)\\\\n).*").Matches(text);
            if (c.Count == 1 && c[0].Success)
            {
                delimiters = new Dictionary<string, TokenType> { { c[0].Groups["delimiter"].Value, TokenType.PlusOperator } };
                offset = c[0].Groups["custom"].Length;
                return true;
            }

            //for delimiters of any length
            c = new Regex("(?<custom>^//(?<item>\\[(?<delimiter>[^\\[\\]]*)\\])+\\\\n).*").Matches(text);
            if (c.Count == 1 && c[0].Success)
            {
                var enu = c[0].Groups["delimiter"].Captures.GetEnumerator();
                delimiters = new Dictionary<string, TokenType>();
                while (enu.MoveNext())
                {
                    var de = (enu.Current as Capture).Value;
                    if (!delimiters.ContainsKey(de))
                    {
                        AssertDelimiterNotOverlapped(de, delimiters);
                        delimiters.Add(de, TokenType.PlusOperator);
                    }
                }

                offset = c[0].Groups["custom"].Length;
                return true;

            }

            // for +,-,*, /
            c = new Regex("(?<custom>^//((?<item>[\\+\\-\\*\\/])\\[(?<delimiter>[^\\[\\]]*)\\])+\\\\n).*").Matches(text);
            if (c.Count == 1 && c[0].Success)
            {
                delimiters = new Dictionary<string, TokenType>();

                var itemEnu = c[0].Groups["item"].Captures.GetEnumerator();
                var deEnu = c[0].Groups["delimiter"].Captures.GetEnumerator();

                delimiters = new Dictionary<string, TokenType>();

                while (itemEnu.MoveNext() && deEnu.MoveNext())
                {
                    var item = (itemEnu.Current as Capture).Value;
                    var de = (deEnu.Current as Capture).Value;

                    AssertDelimiterNotOverlapped(de, delimiters);

                    switch (item)
                    {
                        case "+":
                            delimiters.Add(de, TokenType.PlusOperator);
                            break;
                        case "-":
                            delimiters.Add(de, TokenType.MinusOperator);
                            break;
                        case "*":
                            delimiters.Add(de, TokenType.MultiplyOperator);
                            break;
                        case "/":
                            delimiters.Add(de, TokenType.DivideOperator);
                            break;
                        default:
                            break;

                    }

                }

                offset = c[0].Groups["custom"].Length;
                return true;

            }

            return false;
        }

        private void AssertDelimiterNotOverlapped(string delimiter, Dictionary<string, TokenType> delimiters)
        {
            var enu = delimiters.Keys.GetEnumerator();
            while (enu.MoveNext())
            {
                var overlapped = enu.Current.Length > delimiter.Length ? enu.Current.Contains(delimiter) : delimiter.Contains(enu.Current);
                if (overlapped)
                {
                    throw new OverlappingDelimitersException(new List<string> { delimiter, enu.Current });
                }
            }
        }
    }
}
