using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGP.LexicalAnalysis
{
    /// <summary>
    /// A lexical token which has been scanned.
    /// </summary>
    public struct LexicalScanToken
    {
        /// <summary>
        /// The reference to the original scan token.
        /// </summary>
        public LexicalToken Token;

        /// <summary>
        /// The key of the token, as per the reference Token field.
        /// </summary>
        public string Key
        {
            get
            {
                return Token.Key;
            }
        }

        /// <summary>
        /// The captured string when scanned, if the token specifies to keep the string.
        /// </summary>
        public string ScannedString;

        /// <summary>
        /// Constructs a scan token formed by the original token using the scanned string.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="scannedString"></param>
        public LexicalScanToken(LexicalToken token, string scannedString) {
            Token = token;
            ScannedString = scannedString;
        }

    }
}
