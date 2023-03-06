# Custom Grammar Processor (CGP)
## Features
* Ability to compile a list of regular expressions (not to be confused with regex) - see RegularExpression class
* Combine regular expressions into a LexicalTokenDictionary by wrapping them into the LexicalToken structure.
* Lexical analysis using regular expressions - see usage.

## Usage
1. Create new token dictionary
```
LexicalTokenDictionary dict = new LexicalTokenDictionary();
```
2. Setup lexical tokens for this dictionary.
```
dict.Add(new LexicalToken("Number", "Digit Digit*") { GenericCapture = false }); 
dict.Add(new LexicalToken("Digit", "\"0\" | \"1\" | \"2\" | \"3\" | \"4\" | \"5\" | \"6\" | \"7\" | \"8\" | \"9\""));
dict.Add(new LexicalToken("KW_IF", "\"if\""));
dict.Add(new LexicalToken("KW_DO", "\"do\""));
/*OR*/
dict.Import(new string[] { ... });
/*OR*/
dict.ImportFile(file);
```
Take note the GenericCapture variable. 
On default (true), the only thing being scanned in lexical analysis is the token names (such as keywords).
However, things like numbers and identifiers are non-generic as the number and identifier must be kept for further use.

3. Create expression references which link all nested expressions in the dictionary.
```
dict.CreateExpressionReferences();
```
4. Create a ScannedTokenSequence by using any of the two functions.
```
string text = "if 102 9283 do 1982 if 28372 do 283";
ScannedTokenSequence sequence = dict.Analysis(text);
/*OR*/
ScannedTokenSequence sequence = ScannedTokenSequence.CreateFrom(dict, text);
```

## Token Expression File
Basic Syntax per line:
[!] TOKEN_KEY -> TOKEN_RULE
The exclamation mark (!) is specified only if you want to keep the text that is captured in the scan (i.e. if you need it in further steps).
Use ! for identifiers, numbers, and things with no exact sequence of strings.
TOKEN_KEY is any single word (no strict case)
TOKEN_RULE adheres to the following rules for regular expressions:
* a string in quotes such as "white"
* a sequence of regular expressions representing their concatenation such as: RegExp1 RegExp2 RegExp3
* a set of valid alternative regular expressions for this regular expression seperated by | such as: RegExp1 | RegExp2.
* a regular expression in parentheses, "(" and ")" indicating grouping.
* a regular expression in square brackets, "[" and "]" indicating optional capturing.
* a regular expression followed by * to indicate zero or more occurences.

## Changelog
* Fix some more issues regarding regular expression compiling.
* Added optional support for regular expressions (syntax [ exp ])
* Added in-built Digit, Alpha, and Alphanumerical Expressions that can be linked via the Token Dictionary.