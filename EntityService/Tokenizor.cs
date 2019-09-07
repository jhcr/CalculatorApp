using System;
using System.Collections.Generic;

namespace CalculatorApp.EntityService
{
    /// <summary>
    /// Identify lexical units for a specific language
    /// </summary>
    public class Tokenizor: ITokenizer
    {
        private Dictionary<string, TokenType> _rules;

        public Tokenizor() {
            ApplyDefaultConfig();
        }

        public bool TryParseDelimiter(string lex, out string delimiter, out string literal)
        {
            delimiter = string.Empty;
            literal = string.Empty;

            if (string.IsNullOrWhiteSpace(lex))
                return false;

            foreach (var de in _rules.Keys)
            {
                if (lex.EndsWith(de))
                {
                    delimiter = de;
                    literal = lex.Remove(lex.Length - delimiter.Length);
                    return true;
                }
                  
            }

            return false;
        }

        /// <summary>
        /// Identify token based in simple rules
        /// </summary>
        /// <param name="lex"></param>
        /// <returns></returns>
        public Token Identify(string lex)
        {
            if (_rules.ContainsKey(lex))
            {   // Operator
                return new Token(_rules[lex], lex);
            }
            else if (int.TryParse(lex, out var num))
            {   // Number
                if (num > 1000)
                {
                    return new Token(TokenType.IgnoredNumber, lex);
                }
                else
                {
                    return new Token(TokenType.Number, lex, num);
                }

            }
            else
            {   // Invalid/Missing numbers should be converted to 0
                return new Token(TokenType.Number, lex, 0);
            } 
        }

        /// <summary>
        /// Apply default delimiter config
        /// </summary>
        public void ApplyDefaultConfig()
        {
            _rules = new Dictionary<string, TokenType>() {
                { ",", TokenType.PlusOperator },
                { "\\n", TokenType.PlusOperator }
            };
        }

        /// <summary>
        /// Apply one config on top of default
        /// </summary>
        /// <param name="delimiter"></param>
        /// <param name="tokenType"></param>
        public void ApplyConfig(string delimiter, TokenType tokenType)
        {
            if (string.IsNullOrEmpty(delimiter))
                throw new ArgumentNullException(nameof(delimiter));

            if (_rules.ContainsKey(delimiter))
            {
                _rules[delimiter] = tokenType;
            }
            else
            {
                AssertDelimiterNotOverlapped(delimiter);
                _rules.Add(delimiter, tokenType);
            }
        }

        /// <summary>
        /// Apply on top of default
        /// </summary>
        /// <param name="delimiters"></param>
        public void ApplyConfig(IDictionary<string, TokenType> delimiters)
        {
            if (delimiters == null)
                throw new ArgumentNullException(nameof(delimiters));

            foreach (var de in delimiters.Keys)
            {
                ApplyConfig(de, delimiters[de]);
            }
        }

        private void AssertDelimiterNotOverlapped(string delimiter)
        {
            var enu = _rules.Keys.GetEnumerator();
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
