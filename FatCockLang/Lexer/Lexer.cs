using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Lexer
{
    public List<Token> Tokenize(string input)
    {
        List<Token> tokens = new List<Token>();
        string[] lines = input.Split('\n');

        int curLine = 0;

        // parse each line individually
        foreach (string line in lines)
        {
            curLine++;

            // split on tokens in string
            string[] words = TokenizeString(line.Trim()).ToArray();

            int curToken = 0;
            foreach (string word in words)
            {
                Console.WriteLine($"{curLine} {curToken}: {word}");
                curToken++;

                Token token = new Token();
                token.Line = curLine;
                token.Value = word;

                // token stuff

                // keyword stuff
                if (IsKeyword(word))
                    token.Type = TokenType.Keyword;

                // import
                else if (word == "import")
                    token.Type = TokenType.Import;

                // from (used in import, for now)
                else if (word == "from")
                    token.Type = TokenType.From;

                // open bracket
                else if (word == "(")
                    token.Type = TokenType.OpenParenthesis;

                // close bracket
                else if (word == ")")
                    token.Type = TokenType.CloseParenthesis;

                // open brace
                else if (word == "{")
                    token.Type = TokenType.OpenBrace;

                // close brace
                else if (word == "}")
                    token.Type = TokenType.CloseBrace;

                // string
                else if (Regex.IsMatch(word, "\".*\""))
                    token.Type = TokenType.StringLiteral;

                else if (string.IsNullOrEmpty(word))
                    continue; // skip this step

                // other literals
                else
                    token.Type = TokenType.Identifier;

                tokens.Add(token);
            }
        }

        // return the tokens
        return tokens;
    }

    List<string> TokenizeString(string input)
    {
        string pattern = @"(\""[^\""]*\"")|\b\w+\b|[\{\}\(\)\[\];]|.|\s+";

        List<string> tokens = new List<string>();

        foreach (Match match in Regex.Matches(input, pattern))
        {
            if (string.IsNullOrEmpty(match.Value.Trim()))
                continue;

            tokens.Add(match.Value);
        }

        return tokens;
    }

    // temp setup
    string[] keywords = new string[] { "void", "return" };

    private bool IsKeyword(string word)
    {
        return Array.Exists(keywords, k => k == word);
    }
}