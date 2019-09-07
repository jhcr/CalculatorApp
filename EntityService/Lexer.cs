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

        public Lexer(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        public IEnumerable<Token> Scan(string source)
        {
            if (source == null) // Empty or whitespcae is valid
                throw new ArgumentNullException(nameof(source));

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

            AssertNoNegativeNumber(tokens);

            return tokens;
        }

        public void ApplyDefaultConfig()
        {
            _tokenizer.ApplyDefaultConfig();
        }

        public void ApplyConfig(IDictionary<string, TokenType> delimiters)
        {
            _tokenizer.ApplyConfig(delimiters);
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
