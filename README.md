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
```
Take note the GenericCapture variable. 
On default (true), the only thing being scanned in lexical analysis is the token names (such as keywords).
However, things like numbers and identifiers are non-generic as the number and identifier must be kept for further use.
To be further specified as ! in the token specification file : !(token name) -> (rule)

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

## Changelog
* Changed README.md format.
* Renamed LexicalSequence to ScannedTokenSequence
* Added Analysis function to LexicalTokenDictionary for ease of use. 
* Added additional constructor to lexical token to support string expression usage.