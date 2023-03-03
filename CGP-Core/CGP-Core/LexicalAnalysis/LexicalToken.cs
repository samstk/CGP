﻿using CGP.RegEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGP.LexicalAnalysis
{
    /// <summary>
    /// A lexical token as defined by:
    /// KW_IF -> "if"
    /// 
    /// Each lexical token represents a code that is used to serialize the lexical dictionary.
    /// </summary>
    public struct LexicalToken
    {
        /// <summary>
        /// The key of the token that is further used in parsing.
        /// </summary>
        public string Key;
        /// <summary>
        /// The regular expression that represents how this token is formed using a string.
        /// </summary>
        public RegularExpression Expression;
        /// <summary>
        /// The code of the token.
        /// A lexical token sequence can be exported using this byte code to shorten the file length.
        /// </summary>
        public short Code;
        /// <summary>
        /// Constructs a lexical token from the given regular expression, key, and code.
        /// </summary>
        /// <param name="key">the key of the token</param>
        /// <param name="expression">the string-capturing regular expression for this token</param>
        /// <param name="code">the code of the token</param>
        public LexicalToken(string key, RegularExpression expression, short code)
        {
            Key = key;
            Expression = expression;
            Code = code;
        }
        /// <summary>
        /// Constructs a lexical token from the given regular expression and code, using the key of the expression.
        /// </summary>
        /// <param name="expression">the string-capturing regular expression for this token</param>
        /// <param name="code">the code of the token</param>
        public LexicalToken(RegularExpression expression, short code) : this(expression.Key, expression, code) { }
    }
}
