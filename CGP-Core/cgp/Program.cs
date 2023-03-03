using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGP.LexicalAnalysis;
using CGP.RegEx;

namespace cgp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                LexicalTokenDictionary dict = new LexicalTokenDictionary();
                //dict.ImportFile(args[0]);
                RegularExpression exp = new RegularExpression("Digit", "\"0\" | \"1\" | \"2\" | \"3\" | \"4\" | \"5\" | \"6\" | \"7\" | \"8\" | \"9\"");
                dict.Add(new LexicalToken("Digit", exp, 0));
                exp = new RegularExpression("Number", "Digit Digit*");
                dict.Add(new LexicalToken("Number", exp, 1));
                dict.CreateExpressionReferences();
                
            }
        }
    }
}
