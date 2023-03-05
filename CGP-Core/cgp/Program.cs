using System;
using System.Collections.Generic;
using System.IO;
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
            /*
             * Usage
             * cgp [-t tokenExpressionFile] [-f *.filetype directory / -f file] [-F *.filetype directory] [-s scantext]
             * 
             * [-t tokenExpressionFile] - loads the tokens and expressions from the given file.
             * [-f *.filetype directory / -f file] - recursively adds all files matching the pattern, or a single file.
             * [-F *.filetype directory] - adds all files in the top directory specified matching the pattern.
             * [-s scantext] - adds additional scanning text in this compilation.
             */
            string tokenFile = null;
            List<string> textFiles = new List<string>();
            List<string> scanTexts = new List<string>();
            int mode = 0;
            string wildCard = null;
            foreach(string arg in args)
            {
                if (mode == 0)
                {
                    if (arg == "-t")
                    {
                        mode = 1;
                    }
                    else if(arg == "-f")
                    {
                        mode = 2;
                    }
                    else if (arg == "-f")
                    {
                        mode = 4;
                    }
                    else if (arg == "-s")
                    {
                        mode = 6;
                    }
                }
                else if(mode == 1)
                {
                    tokenFile = arg;
                    mode = 0;
                }
                else if (mode == 2)
                {
                    if (arg.StartsWith("*."))
                    {
                        mode = 3;
                        wildCard = arg;
                    }
                }
                else if (mode == 3)
                {
                    textFiles.AddRange(Directory.GetFiles(arg, wildCard, SearchOption.AllDirectories));
                }
                else if (mode == 4)
                {
                    if (arg.StartsWith("*."))
                    {
                        mode = 5;
                        wildCard = arg;
                    }
                }
                else if (mode == 5)
                {
                    textFiles.AddRange(Directory.GetFiles(arg, wildCard, SearchOption.TopDirectoryOnly));
                }
                else
                {
                    scanTexts.Add(arg);
                }
            }

            if (tokenFile == null) /* Test Case */
            {
                LexicalTokenDictionary dict = new LexicalTokenDictionary();
                
                dict.Add(new LexicalToken("Number", "Digit Digit*") { GenericCapture = false });
                dict.Add(new LexicalToken("Digit", "\"0\" | \"1\" | \"2\" | \"3\" | \"4\" | \"5\" | \"6\" | \"7\" | \"8\" | \"9\""));
                dict.Add(new LexicalToken("KW_IF", "\"if\""));
                dict.Add(new LexicalToken("KW_DO", "\"do\""));
                dict.CreateExpressionReferences();

                ScannedTokenSequence sequence = dict.Analysis("if 1 do 1 2 3 if 4 do 3 2 1 if 5 do 0");

                Console.WriteLine(sequence.ToString());
                Console.ReadKey();
            }
            else
            {
                LexicalTokenDictionary dict = new LexicalTokenDictionary();
                dict.ImportFile(tokenFile);
                dict.CreateExpressionReferences();

                Dictionary<string, ScannedTokenSequence> sequences = new Dictionary<string, ScannedTokenSequence>();
                int scanIndex = 0;
                foreach(string scan in scanTexts)
                {
                    sequences.Add($"*scanArgs[{scanIndex}]", dict.Analysis(scan));
                    scanIndex++;
                }

                foreach(string file in textFiles)
                {
                    sequences.Add(file, dict.Analysis(File.ReadAllText(file)));
                }

                /* For now just output as further operations not implemented. */

                foreach(string key in sequences.Keys)
                {
                    Console.WriteLine("$ " + key);
                    Console.WriteLine(sequences[key].ToString());
                }

                Console.ReadKey();
            }
            
        }
    }
}
