﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CalculatorApp.EntityService
{
    public enum TokenType
    {
        None = 0,
        PlusOperator = 1,
        MinusOperator = 2,
        MultiplyOperator = 3,
        DivideOperator = 4,
        Number = 5,
        IgnoredNumber = 6
    }

    /// <summary>
    /// A lexical unit with identifed meaning
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Identified type
        /// </summary>
        public TokenType Type { get; set; }
        /// <summary>
        /// Raw value
        /// </summary>
        public string Raw { get; set; }
        /// <summary>
        /// Identified value if is a number
        /// </summary>
        public int? Value { get; set; }

        public Token() { }

        public Token(TokenType type, string raw, int? value = null)
        {
            Type = type;
            Raw = raw;
            Value = value;
        }
    }
}
