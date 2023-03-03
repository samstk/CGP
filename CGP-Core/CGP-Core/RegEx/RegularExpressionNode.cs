using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace CGP.RegEx
{
    /// <summary>
    /// A regular expression node formed from a rule.
    /// </summary>
    public sealed class RegularExpressionNode
    {
        /// <summary>
        /// Specifies whether this node takes zero or more occurences.
        /// </summary>
        public bool ZeroOrMoreOccurences 
        { get; private set; } = false;

        /// <summary>
        /// A list of alternative nodes in this node, as specified by | in the rules.
        /// </summary>
        public LinkedList<RegularExpressionNode> AlternativeNodes 
        { get; private set; } = new LinkedList<RegularExpressionNode>();

        /// <summary>
        /// A sequences of nodes that specifies this how group captures strings.
        /// </summary>
        public LinkedList<RegularExpressionNode> Sequence
        { get; private set; } = new LinkedList<RegularExpressionNode>();

        /// <summary>
        /// If set, this node represents a simple string requirement.
        /// </summary>
        public string CharacterSequence { get; private set; } = null;

        /// <summary>
        /// If set, this node represents a link to another regular expression.
        /// </summary>
        public string ReferenceExpressionKey { get; private set; } = null;
        /// <summary>
        /// If ReferenceExpressionKey is set, and the expressions are linked, then this is a
        /// reference to the expression used for this node.
        /// </summary>
        public RegularExpression ReferenceExpression { get; private set; } = null;


        /// <summary>
        /// Adds a group to the sequence of the current node.
        /// Brackets should be removed from the group rule before calling this function.
        /// </summary>
        /// <param name="baseExpression">The base expression of the node</param>
        /// <param name="rule">The rule of the group</param>
        /// <returns></returns>
        private RegularExpressionNode AddGroupToSequence(RegularExpression baseExpression, string rule)
        {
            RegularExpressionNode node = new RegularExpressionNode(baseExpression, rule);
            Sequence.AddLast(node);
            return node;
        }
        /// <summary>
        /// Adds a token to the sequence of the current node.
        /// </summary>
        /// <param name="baseExpression">The base expression of the node</param>
        /// <param name="rule">The rule of the group</param>
        /// <returns></returns>
        private RegularExpressionNode AddTokenToSequence(RegularExpression baseExpression, string rule)
        {
            if (rule[0] == '\"')
            {
                RegularExpressionNode node = new RegularExpressionNode();
                node.CharacterSequence = rule.Substring(1, rule.Length-1);
                Sequence.AddLast(node);
                return node;
            }
            else
            {
                RegularExpressionNode node = new RegularExpressionNode();
                node.ReferenceExpressionKey = rule;
                if(rule == baseExpression.Key)
                {
                    node.ReferenceExpression = baseExpression;
                }
                Sequence.AddLast(node);
                return node;
            }
        }

        /// <summary>
        /// Creates a regular expression node without a rule.
        /// </summary>
        private RegularExpressionNode()
        {

        }
        /// <summary>
        /// Constructs a regular expression node from a rule.
        /// </summary>
        /// <param name="rule">a rule that follows the Regular Expression syntax as outlined in RegularExpression</param>
        public RegularExpressionNode(RegularExpression baseExpression, string rule)
        {
            /* Split the rule into alternative groupings */
            LinkedList<string> versions = new LinkedList<string>();

            bool hasAlternativeNodes = false;
            
            int mode = 0; // 0 = Continue, 1 = In String, 2 = In String (escape), 3+ = In String(escape \u0000)
            int brackets = 0; // To avoid inner groups while splitting, only split when brackets=0
            string versionStringBuilder = "";
            for (int i = 0; i < rule.Length; i++) {
                char c = rule[i];
                bool addchar = true;
                if (mode == 0)
                {
                    if (c == '\"')
                    {
                        mode = 1;
                    }
                    else if(c=='|' && brackets == 0)
                    {
                        addchar = false;
                        versions.AddLast(versionStringBuilder.Trim());
                        versionStringBuilder = "";
                        hasAlternativeNodes = true;
                    }
                    else if (c == '(')
                    {
                        brackets++;
                    }
                    else if (c == ')')
                    {
                        brackets--;
                    }
                }
                else if (mode == 1)
                {
                    if (c == '\"')
                    {
                        mode = 0;
                    }
                    else if (c == '\\')
                    {
                        mode = 2;
                    }
                }
                else if (mode == 2)
                {
                    if (c == 'u')
                    {
                        mode = 3;
                    }
                    else
                    {
                        mode = 1;
                    }
                }
                else if (mode >= 3)
                {
                    mode++;
                    if (mode == 7)
                    {
                        mode = 1;
                    }
                }
                if (addchar)
                {
                    versionStringBuilder += c;
                }
            }
            if (hasAlternativeNodes)
            {
                versions.AddLast(versionStringBuilder.Trim());
                versionStringBuilder = null;
                // Split into nodes
                foreach (string alt in versions)
                {
                    AlternativeNodes.AddLast(new RegularExpressionNode(baseExpression, alt));
                }
            }
            else
            {
                versionStringBuilder = null;
                RegularExpressionNode lastSequenceNode = null;
                string nodeBuilder = "";
                mode = 0;
                brackets = 0;

                for (int i = 0; i < rule.Length; i++)
                {
                    char c = rule[i];
                    bool addchar = true;
                    if (mode == 0)
                    {
                        if (char.IsWhiteSpace(c) || c == '*')
                        {
                            nodeBuilder = nodeBuilder.Trim();
                            if (nodeBuilder.Length > 0)
                            {
                                lastSequenceNode = AddTokenToSequence(baseExpression, nodeBuilder);
                                nodeBuilder = "";
                            }
                            if (c == '*')
                            {
                                addchar = false;
                                if (lastSequenceNode == null)
                                {
                                    throw new Exception("'*' operator is not placed after group or token.");
                                }
                                lastSequenceNode.ZeroOrMoreOccurences = true;
                            }
                        }
                        else if (c == '\"')
                        {
                            mode = 1;
                        }
                        else if (c == '(')
                        {
                            if (brackets == 0)
                            {
                                nodeBuilder = nodeBuilder.Trim();
                                if(nodeBuilder.Length > 0)
                                {
                                    lastSequenceNode = AddTokenToSequence(baseExpression, nodeBuilder.Trim());
                                    nodeBuilder = "";
                                }
                                addchar = false;
                            }
                            brackets++;
                            
                        }
                        else if (c == ')')
                        {
                            brackets--;
                            if (brackets == 0)
                            {
                                addchar = false;
                                nodeBuilder = nodeBuilder.Trim();
                                lastSequenceNode = AddGroupToSequence(baseExpression, nodeBuilder.Trim());
                                nodeBuilder = "";
                            }
                        }
                    }
                    else if (mode == 1)
                    {
                        if (c == '\"')
                        {
                            mode = 0;
                            lastSequenceNode = AddTokenToSequence(baseExpression, nodeBuilder.Trim());
                            nodeBuilder = "";
                            addchar = false;
                        }
                        else if (c == '\\')
                        {
                            mode = 2;
                        }
                    }
                    else if (mode == 2)
                    {
                        if (c == 'u')
                        {
                            mode = 3;
                        }
                        else
                        {
                            mode = 1;
                        }
                    }
                    else if (mode >= 3)
                    {
                        mode++;
                        if (mode == 7)
                        {
                            mode = 1;
                        }
                    }
                    if (addchar)
                    {
                        nodeBuilder += c;
                    }
                }

                nodeBuilder = nodeBuilder.Trim();
                if(nodeBuilder.Length > 0)
                {
                    AddTokenToSequence(baseExpression, nodeBuilder);
                }
            }
        }
        /// <summary>
        /// Links all instances of external regular expressions to an active object.
        /// </summary>
        /// <param name="linkingFunction">A function that acts as a dictionary for external expression keys</param>
        public void Link(RegularExpressionLinkDelegate linkingFunction)
        {
            foreach (RegularExpressionNode node in AlternativeNodes)
                node.Link(linkingFunction);

            foreach (RegularExpressionNode node in Sequence)
                node.Link(linkingFunction);

            if (ReferenceExpressionKey != null)
                ReferenceExpression = linkingFunction(ReferenceExpressionKey);
        }

        /// <summary>
        /// Determines if two strings match within the indices given
        /// </summary>
        /// <param name="text">first text to compare</param>
        /// <param name="text2">second text to compare</param>
        /// <param name="start1">start index of the first text</param>
        /// <param name="start2">start index of the second text</param>
        /// <param name="length">length of the match</param>
        /// <returns></returns>
        private static bool StringMatches(string text1, int start1, string text2, int start2, int length)
        {
            if (start1 + length > text1.Length || start2 + length > text2.Length)
                return false;
            for (int i = 0; i<length; i++)
            {
                if (text1[start1+i] != text2[start2+i])
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Attempts to capture a part of the text using this regular expression's rules from the start index to the end (exclusive).
        /// This function will return -1 if the text starting from the start index is incompatible with these rules.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public int Capture(string text, int start, int end)
        {
            if(CharacterSequence != null)
            {
                if (start + CharacterSequence.Length > end)
                    return -1;

                if(StringMatches(text, start, CharacterSequence, 0, CharacterSequence.Length))
                {
                    return start + CharacterSequence.Length;
                }

                return -1;
            }
            if(ReferenceExpressionKey != null)
            {
                if (ReferenceExpression == null)
                    throw new Exception("Expression requires linking");
                return ReferenceExpression.Capture(text, start, end);
            }

            if (AlternativeNodes.Count > 0)
            {
                foreach (RegularExpressionNode node in AlternativeNodes)
                {
                    int cap = node.Capture(text, start, end);
                    if (cap != -1)
                        return cap;
                }
                return -1;
            }

            if (Sequence.Count > 0)
            {
                int startPoint = start;
                foreach(RegularExpressionNode node in Sequence)
                {
                    int cap = node.Capture(text, startPoint, end);
                    if (cap == -1)
                    {
                        if (node.ZeroOrMoreOccurences)
                            continue;
                        else
                        {
                            return -1;
                        }
                    }
                    else if (node.ZeroOrMoreOccurences)
                    {
                        while (cap != -1)
                        {
                            startPoint = cap;
                            cap = node.Capture(text, startPoint, end);
                        }
                    }
                    else startPoint = cap;
                }
                return startPoint;
            }
            throw new Exception("Unhandled regular expression rule");
        }
    }
}
