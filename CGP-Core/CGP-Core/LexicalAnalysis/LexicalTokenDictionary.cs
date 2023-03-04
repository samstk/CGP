using CGP.RegEx;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CGP.LexicalAnalysis
{
    /// <summary>
    /// A dictionary containing a list of lexical tokens for further parsing.
    /// A lexical token can have a maximum character length of 255.
    /// 
    /// All 
    /// </summary>
    public sealed class LexicalTokenDictionary
    {
        /// <summary>
        /// A dictionary linking a sequence of integer codes to a lexical token.
        /// </summary>
        private Dictionary<short, LexicalToken> CodeDictionary = new Dictionary<short, LexicalToken>();
        /// <summary>
        /// A dictionary linking a sequence of keys to a lexical token.
        /// </summary>
        private Dictionary<string, LexicalToken> KeyDictionary = new Dictionary<string, LexicalToken>();
        /// <summary>
        /// A linked list in sorted order of largest tokens to smallest tokens.
        /// </summary>
        private LinkedList<LexicalToken> Tokens = new LinkedList<LexicalToken>();
        /// <summary>
        /// Specifies whether to keep whitespace tokens when scanning for a lexical token sequence.
        /// Used in LexicalSequence.CreateFrom(..);
        /// </summary>
        public bool KeepWhitespaceTokens = false;
        /// <summary>
        /// Specifies whether to keep tab tokens when scanning for a lexical token sequence.
        /// Used in LexicalSequence.CreateFrom(..);
        /// </summary>
        public bool KeepTabTokens = false;
        /// <summary>
        /// Specifies whether to keep newline tokens when scanning for a lexical token sequence.
        /// Used in LexicalSequence.CreateFrom(..);
        /// </summary>
        public bool KeepNewlineTokens = false;

        /// <summary>
        /// Gets a lexical token to this dictionary.
        /// </summary>
        /// <param name="code">The key of the token</param>
        /// <returns></returns>
        /// <exception cref="Exception">An exception which occurs when an unexpected code is used</exception>
        public LexicalToken this[string key]
        {
            get
            {
                if (!KeyDictionary.ContainsKey(key))
                {
                    throw new Exception("Unexpected key");
                }
                return KeyDictionary[key];
            }
        }

        /// <summary>
        /// Gets a lexical token to this dictionary.
        /// </summary>
        /// <param name="code">The code of the token</param>
        /// <returns></returns>
        /// <exception cref="Exception">An exception which occurs when an unexpected code is used</exception>
        public LexicalToken this[short code]
        {
            get
            {
                if (!CodeDictionary.ContainsKey(code))
                {
                    throw new Exception("Unexpected code");
                }
                return CodeDictionary[code];
            }
        }

        /// <summary>
        /// Adds a new token to the lexical dictionary.
        /// </summary>
        /// <param name="token">a lexical token to be included in this dictionary</param>
        /// <exception cref="Exception">An exception which occurs when the token code or characters already exist in </exception>
        public void Add(LexicalToken token)
        {
            /* Pre-liminary check for existing codes or characters */
            token.Code = (short)CodeDictionary.Count;
            //if (CodeDictionary.ContainsKey(token.Code))
            //{
            //    throw new Exception("Dictionary already contains token");
            //}
            if (KeyDictionary.ContainsKey(token.Key))
            {
                throw new Exception("Dictionary already contains token");
            }

            /* Add into dictionary and list */
            CodeDictionary.Add(token.Code, token);
            KeyDictionary.Add(token.Key, token);
            Tokens.AddLast(token);
        }

        /// <summary>
        /// Gets an array of all tokens in this dictionary.
        /// </summary>
        /// <returns></returns>
        public LexicalToken[] GetTokens()
        {
            LexicalToken[] tokens = new LexicalToken[CodeDictionary.Count];
            int i = 0;
            foreach(LexicalToken token in Tokens)
            {
                tokens[i] = token;
                i++;
            }
            return tokens;
        }


        /// <summary>
        /// Reads and import lexical token rules. from a regular expression file (.rgx)
        /// 
        /// </summary>
        /// <param name="file">a file name to be read from</param>
        public void ImportFile(string file)
        {
            Import(File.ReadAllLines(file));
        }

        /// <summary>
        /// Reads and import lexical token rules from a list of lines in regular expression format.
        /// </summary>
        public void Import(string[] lines)
        {
            throw new Exception("Not implemented.");
        }

        /// <summary>
        /// Gets the regular expression behind a lexical token given by the key.
        /// </summary>
        /// <param name="key">the key of the token</param>
        /// <returns></returns>
        public RegularExpression GetExpression(string key)
        {
            return this[key].Expression;
        }

        /// <summary>
        /// Creates references from tokens to tokens by using the linking function of all regular expressions.
        /// </summary>
        public void CreateExpressionReferences()
        {
            foreach(LexicalToken token in Tokens)
            {
                token.Expression.Link(GetExpression);
            }
        }

        /// <summary>
        /// An global token specified for scans representing whitespace characters.
        /// </summary>
        public static LexicalToken WhitespaceToken { get; private set; }
            = LexicalToken.CreateEmpty("WHITESPACE", -1);

        /// <summary>
        /// An global token specified for scans representing tab characters.
        /// </summary>
        public static LexicalToken TabToken { get; private set; }
            = LexicalToken.CreateEmpty("TAB", -2);

        /// <summary>
        /// An global token specified for scans representing newlines (\r\n, \r \n).
        /// </summary>
        public static LexicalToken NewlineToken { get; private set; }
            = LexicalToken.CreateEmpty("NEWLINE", -3);

        /// <summary>
        /// An global token specified for scans representing unscannable strings.
        /// </summary>
        public static LexicalToken ReservedToken { get; private set; }
            = LexicalToken.CreateEmpty("RESERVED", -4);


        /// <summary>
        /// Scans the next suitable token from the start index.
        /// 
        /// Follows the following rules:
        /// 1. Scans for the token which includes the most characters (longest string).
        /// 2. If the scan length is equal, always chooses the earliest added token to base the scan from.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <returns>a tuple of the next search (start) index and the scanned token</returns>
        public (int from, int next, LexicalToken token) ScanNext(string text, int startIndex)
        {
            while (startIndex < text.Length)
            {
                char startChar = text[startIndex];

                if (KeepTabTokens)
                {
                    if (startChar == '\t')
                    {
                        return (startIndex, startIndex + 1, TabToken);
                    }
                }
                else
                {
                    if (startChar == '\t')
                    {
                        startIndex++;
                        continue;
                    }
                }
                if (KeepNewlineTokens)
                {
                    if (startChar == '\r')
                    {
                        if (startIndex + 1 < text.Length)
                        {
                            if (text[startIndex + 1] == '\n')
                                return (startIndex, startIndex + 2, NewlineToken);
                        }
                        return (startIndex, startIndex + 1, NewlineToken);
                    }
                    else if (startChar == '\n')
                    {
                        return (startIndex, startIndex + 1, NewlineToken);
                    }
                }
                else
                {
                    if (startChar=='\r' || startChar=='\n')
                    {
                        startIndex++;
                        continue;
                    }
                }
                if (KeepWhitespaceTokens)
                {
                    if (char.IsWhiteSpace(startChar))
                    {
                        return (startIndex, startIndex + 1, WhitespaceToken);
                    }
                }
                else
                {
                    if(char.IsWhiteSpace(startChar))
                    {
                        startIndex++;
                        continue;
                    }
                }
                break;
            }
            if(startIndex >= text.Length)
            {
                return (-2, -2, ReservedToken);
            }

            LexicalToken captureToken = ReservedToken;
            int captureLength = -1;
            foreach(LexicalToken token in Tokens)
            {
                int capture = token.Expression.Capture(text, startIndex, text.Length);
                if(capture != -1)
                {
                    int length = capture - startIndex;
                    if(captureLength < length)
                    {
                        captureToken = token;
                        captureLength = length;
                    }
                }
            }
            return (startIndex, captureLength == -1 ? -1 : startIndex + captureLength, captureToken);
        }

        /// <summary>
        /// Analyses the following text by scanning all available tokens from this dictionary.
        /// </summary>
        /// <param name="text">the text to scan.</param>
        /// <returns></returns>
        public ScannedTokenSequence Analysis(string text)
        {
            return ScannedTokenSequence.CreateFrom(this, text);
        }
    }
}
