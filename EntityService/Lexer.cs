using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculatorApp.EntityService
{
    /// <summary>
    /// Scan text into tokens with identified meaning
    /// </summary>
    public class Lexer : ILexer
    {
        ITokenizer _tokenizer;
        IDelimiterParser _parser;
        IConfiguration _config;

        public Lexer(ITokenizer tokenizer, IDelimiterParser parser, IConfiguration config)
        {
            _tokenizer = tokenizer;
            _parser = parser;
            _config = config;
        }

        public IEnumerable<Token> Scan(string source)
        {
            if (source == null) // Empty or whitespcae is valid
                throw new ArgumentNullException(nameof(source));

            // Try parse custom delimiters
            if (_parser.TryParse(source, out var offset, out var delimiters))
            {
                _tokenizer.ApplyCustomRule(delimiters);
                source = source.Remove(0, offset);
            }
            else
            {   // reset custom rule for each scan
                _tokenizer.ResetCustomRule();
            }

            var reader = source.GetEnumerator();
            var lex = string.Empty;
            var tokens = new List<Token>();

            while (reader.MoveNext())
            {
                lex += reader.Current;
                if (_tokenizer.TryParseDelimiter(lex, out var delimiter, out var literal))
                {
                    tokens.Add(_tokenizer.Identify(literal));
                    tokens.Add(_tokenizer.Identify(delimiter));

                    lex = string.Empty;
                    delimiter = string.Empty;
                    literal = string.Empty;
                }
               
            }
            // Token list has to be ended by a non-delimiter lex even the lex is empty.
            tokens.Add(_tokenizer.Identify(lex));

            if(_config.DenyNegativeNumbers)
                AssertNoNegativeNumber(tokens);

            return tokens;
        }

        /// <summary>
        /// Validate numbers are not negative
        /// </summary>
        /// <param name="tokens"></param>
        private void AssertNoNegativeNumber(IEnumerable<Token> tokens)
        {
            var negativeNumbers = tokens.Where(t => t.Type == TokenType.Number && t.Value < 0).Select(t => t.Value.Value);

            if (negativeNumbers.Count() > 0)
            {
                throw new NagativeNumberException(negativeNumbers);
            }
        }
    }
}
