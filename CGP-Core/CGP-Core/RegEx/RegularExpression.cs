using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGP.RegEx
{
    /// <summary>
    /// A delegate function that takes a key and returns a regular expression from that key.
    /// </summary>
    /// <param name="key">The key of the regular expression.</param>
    /// <returns>A regular expression of that key.</returns>
    public delegate RegularExpression RegularExpressionLinkDelegate(string key);

    /// <summary>
    /// A regular expression defined by (Key -> Rules)        
    /// 
    /// Can be used to extract a sequence of characters under the same key.
    /// 
    /// A Regular Expression can either be:
    /// - a string in quotes such as "while"
    /// - a sequence of regular expressions representing their concatenation such as: RegExp1 RegExp2 RegExp3
    /// - a set of valid alternative regular expressions for this regular expression seperated by | such as: RegExp1 | RegExp2.
    /// - A regular expression in Parentheses, "(" and ")" indicating grouping.
    /// - A regular expression followed by * to indicate zero or more occurences.
    /// </summary>
    public sealed class RegularExpression
    {
        /// <summary>
        /// The access key for this regular expression.
        /// </summary>
        public string Key = "";

        /// <summary>
        /// The base node for this regular expression to start from.
        /// </summary>
        public RegularExpressionNode BaseNode;

        /// <summary>
        /// Constructs a regular expression given the rule.
        /// 
        /// A Regular Expression can either be:
        /// - a string in quotes such as "while"
        /// - a sequence of regular expressions representing their concatenation such as: RegExp1 RegExp2 RegExp3
        /// - a set of valid alternative regular expressions for this regular expression seperated by | such as: RegExp1 | RegExp2.
        /// - A regular expression in Parentheses, "(" and ")" indicating grouping.
        /// - A regular expression followed by * to indicate zero or more occurences.
        /// 
        /// External regular expressions can be created, however every regular expression must be linked using the Link function.
        /// </summary>
        /// <param name="rule">A rule to extract characters from a string.</param>
        public RegularExpression(string key, string rule)
        {
            Key = key;
            BaseNode = new RegularExpressionNode(this, rule);
        }
        
        /// <summary>
        /// Links all instances of external regular expressions to an active object.
        /// </summary>
        /// <param name="linkingFunction">A function that acts as a dictionary for external expression keys</param>
        public void Link(RegularExpressionLinkDelegate linkingFunction)
        {
            BaseNode.Link(linkingFunction);
        }

        /// <summary>
        /// Attempts to capture a part of the text using this regular expression's rules from the start index to the end (exclusive).
        /// This function will return -1 if the text starting from the start index is incompatible with these rules.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public int Capture(string text, int start, int end)
        {
            return BaseNode.Capture(text, start, end);
        }

        /// <summary>
        /// Attempts to capture a part of the text using this regular expression's rules.
        /// This function will return -1 if the text starting from index 0 is incompatible with these rules.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public int Capture(string text)
        {
            return Capture(text, 0, text.Length);
        }
    }
}
