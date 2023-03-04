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

                dict.Add(new LexicalToken("Number", "Digit Digit*") { GenericCapture = false }); 
                dict.Add(new LexicalToken("Digit", "\"0\" | \"1\" | \"2\" | \"3\" | \"4\" | \"5\" | \"6\" | \"7\" | \"8\" | \"9\""));
                dict.Add(new LexicalToken("KW_IF", "\"if\""));
                dict.Add(new LexicalToken("KW_DO", "\"do\""));
                dict.CreateExpressionReferences();

                ScannedTokenSequence sequence = dict.Analysis("if 1 do 1 2 3 if 4 do 3 2 1 if 5 do 0");

                Console.WriteLine(sequence.ToString());
                Console.ReadKey();
            }
        }
    }
}
