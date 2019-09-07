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

        public bool TryParseDelimiter(string lex, out string delimitor, out string literal)
        {
            delimitor = string.Empty;
            literal = string.Empty;

            if (string.IsNullOrWhiteSpace(lex))
                return false;

            foreach (var de in _rules.Keys)
            {
                if (lex.EndsWith(de))
                {
                    delimitor = de;
                    literal = lex.Remove(lex.Length - delimitor.Length);
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
        /// <param name="delimitor"></param>
        /// <param name="tokenType"></param>
        public void ApplyConfig(string delimitor, TokenType tokenType)
        {
            if (string.IsNullOrEmpty(delimitor))
                throw new ArgumentNullException(nameof(delimitor));

            if (_rules.ContainsKey(delimitor))
            {
                _rules[delimitor] = tokenType;
            }
            else
            {
                AssertDelimiterNotOverlapped(delimitor);
                _rules.Add(delimitor, tokenType);
            }
        }

        /// <summary>
        /// Apply on top of default
        /// </summary>
        /// <param name="delimitors"></param>
        public void ApplyConfig(IDictionary<string, TokenType> delimitors)
        {
            if (delimitors == null)
                throw new ArgumentNullException(nameof(delimitors));

            foreach (var de in delimitors.Keys)
            {
                ApplyConfig(de, delimitors[de]);
            }
        }

        private void AssertDelimiterNotOverlapped(string delimitor)
        {
            var enu = _rules.Keys.GetEnumerator();
            while (enu.MoveNext())
            {
                var overlapped = enu.Current.Length > delimitor.Length ? enu.Current.Contains(delimitor) : delimitor.Contains(enu.Current);
                if (overlapped)
                {
                    throw new OverlappingDelimitorsException(new List<string> { delimitor, enu.Current });
                }
            }
        }
    }
}
