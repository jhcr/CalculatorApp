using System;
using System.Collections.Generic;

namespace CalculatorApp.EntityService
{
    /// <summary>
    /// Identify lexical units for a specific language
    /// </summary>
    public class Tokenizor: ITokenizer
    {
        IConfiguration _config;
        private IDictionary<string, TokenType> _rules;

        public Tokenizor(IConfiguration config) {
            _config = config;
            _rules = _config.DefaultRule;
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

            if (lex.EndsWith(_config.AlternateDelimiter))
            {
                delimiter = _config.AlternateDelimiter;
                literal = lex.Remove(lex.Length - delimiter.Length);
                return true;
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
            else if (_config.AlternateDelimiter == lex)
            {
                // Operator match alternate delimiter
                return new Token(TokenType.PlusOperator, lex);
            }
            else if (int.TryParse(lex, out var num))
            {   // Number
                if (num > _config.NumberUpperBound)
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
        /// Apply custom delimiters
        /// </summary>
        /// <param name="delimiters"></param>
        public void ApplyCustomRule(IDictionary<string, TokenType> delimiters)
        {
            if (delimiters == null)
                throw new ArgumentNullException(nameof(delimiters));

            foreach (var de in _config.DefaultRule.Keys)
            {
                if (!delimiters.ContainsKey(de))
                {
                    delimiters.Add(de, _rules[de]);
                }
            }

            _rules = delimiters;
        }

        /// <summary>
        /// Remove custom delimiters
        /// </summary>
        /// <param name="delimiters"></param>
        public void ResetCustomRule()
        {
            _rules = _config.DefaultRule;
        }
    }
}
