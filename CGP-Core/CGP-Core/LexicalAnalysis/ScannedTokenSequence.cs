using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGP.LexicalAnalysis
{
    /// <summary>
    /// A sequence consisting of a number of scanned lexical tokens.
    /// </summary>
    public sealed class ScannedTokenSequence
    {
        /// <summary>
        /// The dictionary in which this sequence was formed from.
        /// </summary>
        public LexicalTokenDictionary BaseDictionary { get; private set; }

        /// <summary>
        /// A list of tokens in sequence.
        /// </summary>
        public LinkedList<LexicalScanToken> SequenceTokens { get; private set; } = new LinkedList<LexicalScanToken>();

        /// <summary>
        /// Constructs the lexical sequence using the base dictionary.
        /// </summary>
        /// <param name="baseDictionary"></param>
        public ScannedTokenSequence(LexicalTokenDictionary baseDictionary)
        {
            BaseDictionary = baseDictionary;
        }

        /// <summary>
        /// Appends a scan token based on the original token rules and the scanned string.
        /// </summary>
        /// <param name="token">the token in which the scan was based on</param>
        /// <param name="scannedString">Either the scanned string of the token or NULL if unnecessary.</param>
        public void Append(LexicalToken token, string scannedString)
        {
            SequenceTokens.AddLast(new LexicalScanToken(token, scannedString));
        }

        /// <summary>
        /// Creates a lexical sequence by scanning the given text in the token dictionary.
        /// 
        /// E.g.
        /// 
        /// KW_IF -> "if"
        /// KW_TRUE -> "true"
        /// KW_THEN -> "then"
        /// KW_FALSE -> "false"
        /// 
        /// Scan:
        ///     if true then false 
        /// Results in:
        ///     KW_IF, KW_TRUE, KW_THEN, KW_FALSE
        /// </summary>
        /// <param name="tokenDictionary">the dictionary in which all tokens are defined</param>
        /// <param name="text">the text to scan.</param>
        /// <returns></returns>
        public static ScannedTokenSequence CreateFrom(LexicalTokenDictionary tokenDictionary, string text)
        {
            ScannedTokenSequence seq = new ScannedTokenSequence(tokenDictionary);
            int captureIndex = 0;

            while (captureIndex < text.Length)
            {
                (int from, int next, LexicalToken token) = tokenDictionary.ScanNext(text, captureIndex);
                captureIndex = from; /* For whitespace skipping */
                if (token.Code == -1) /* Whitespace */
                {
                    if (tokenDictionary.KeepWhitespaceTokens)
                    {
                        seq.Append(token, token.GenericCapture ? null : text.Substring(captureIndex, next-captureIndex));
                    }
                }
                else if (token.Code == -2) /* Tab */
                {
                    if (tokenDictionary.KeepTabTokens)
                    {
                        seq.Append(token, token.GenericCapture ? null : text.Substring(captureIndex, next - captureIndex));
                    }
                }
                else if (token.Code == -3) /* Newline */
                {
                    if (tokenDictionary.KeepNewlineTokens)
                    {
                        seq.Append(token, token.GenericCapture ? null : text.Substring(captureIndex, next - captureIndex));
                    }
                }
                else if (token.Code == -4) /* Error */
                {
                    throw new Exception("Unable to find scannable token.");
                }
                else if (token.Code == -5)
                {
                    break;
                }
                else
                {
                    seq.Append(token, token.GenericCapture ? null : text.Substring(captureIndex, next - captureIndex));
                }
                captureIndex = next;
            }

            return seq;
        }

        /// <summary>
        /// Creates a string representation of this sequence.
        /// TOKEN_ID, TOKEN_ID2, TOKEN_ID3 'scannedString', TOKEN_ID4, ...
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string txt = "";
            foreach(LexicalScanToken token in SequenceTokens)
            {
                if (txt.Length > 0)
                    txt += ", ";
                txt += token.Key;
                if (token.ScannedString != null)
                    txt += " " + token.ScannedString;
            }
            return txt;
        }
    }
}
