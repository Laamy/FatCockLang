using System;
using System.Collections.Generic;

class Parser
{
    private List<Token> tokens;
    private int current = 0;

    // Constructor
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public ASTNode Parse()
    {
        return ParseProgram();
    }

    private ASTNode ParseProgram()
    {
        // tree starts with a program node
        var programNode = new ASTNode { Type = "Program" };
        
        while (current < tokens.Count)
        {
            var token = tokens[current];

            if (token.Type == TokenType.Import)
            {
                programNode.Children.Add(ParseImportStatement());
            }
            else if (token.Type == TokenType.Keyword && token.Value == "void") // functions
            {
                programNode.Children.Add(ParseFunctionDeclaration());
            }
            else
            {
                // error handling
                throw new Exception($"Unexpected token {token.Type} at line {token.Line}, (value: {tokens[current].Value}) (current: {current})");
            }
        }

        return programNode;
    }

    private ASTNode ParseFunctionDeclaration()
    {
        var functionNode = new ASTNode { Type = "FunctionDeclaration" };

        if (tokens[current].Value == "void")
        {
            Consume(TokenType.Keyword);
            functionNode.Value = "void";
        }

        functionNode.Children.Add(ParseIdentifier());
        
        // ignore arguments for now (treated as invalid syntax)
        Expect(TokenType.OpenParenthesis);
        Expect(TokenType.CloseParenthesis);

        // code block is here (function body)
        var codeBlockNode = ParseCodeBlock();
        functionNode.Children.Add(codeBlockNode);

        return functionNode;
    }

    private ASTNode ParseCodeBlock()
    {
        var codeBlockNode = new ASTNode { Type = "CodeBlock" };
        Expect(TokenType.OpenBrace);

        while (current < tokens.Count && tokens[current].Type != TokenType.CloseBrace)
        {
            var token = tokens[current];

            if (token.Type == TokenType.Identifier)
            {
                var statementNode = ParseStatement();
                if (statementNode != null)
                {
                    codeBlockNode.Children.Add(statementNode);
                }
            }
            else
            {
                // error handling
                throw new Exception($"Unexpected token {token.Type} in codeblock at line {token.Line}, (value: {tokens[current].Value}) (current: {current})");
            }
        }

        Expect(TokenType.CloseBrace);

        return codeBlockNode;
    }

    private ASTNode ParseStatement()
    {
        var token = tokens[current];

        if (token.Type == TokenType.Identifier)
        {
            return ParseIdentifierStatement();
        }
        else
        {
            throw new Exception($"Unexpected token {token.Type} in statement at line {token.Line}, (value: {tokens[current].Value}) (current: {current})");
        }
    }

    private ASTNode ParseIdentifierStatement()
    {
        var identifierNode = ParseIdentifier();
        var nextToken = PeekNextToken();

        Console.WriteLine($"{identifierNode.Value} {nextToken.Value}");

        if (tokens[current].Type == TokenType.OpenParenthesis)
        {
            var functionCallNode = new ASTNode { Type = "FunctionCall" };
            functionCallNode.Children.Add(identifierNode);
            functionCallNode.Children.Add(ParseFunctionArguments());
            return functionCallNode;
        }
        else if (tokens[current].Value == ".")
        {
            // accessing something inside of a class
            Consume(TokenType.Identifier);

            nextToken = PeekNextToken();

            if (nextToken != null && nextToken.Type == TokenType.OpenParenthesis)
            {
                // we only allow 1 deep for now (TODO: make it recursive)
                var recursiveFunctionCallNode = new ASTNode { Type = "FunctionCall" };
                recursiveFunctionCallNode.Children.Add(identifierNode);
                recursiveFunctionCallNode.Children.Add(ParseIdentifier());
                recursiveFunctionCallNode.Children.Add(ParseFunctionArguments());
                return recursiveFunctionCallNode;
            }
            else
            {
                if (nextToken == null)
                {
                    throw new Exception($"Unexpected end of file in identifier statement at line {tokens[current].Line}, (value: {tokens[current].Value}) (current: {current})");
                }
                else
                {
                    throw new Exception($"Unexpected token {nextToken.Type} in identifier statement at line {nextToken.Line}, (value: {tokens[current].Value}) (current: {current})");
                }
            }
        }
        else
        {
            // no other possible case for now (stuff like variable assignments)
            if (nextToken == null)
            {
                throw new Exception($"Unexpected end of file in identifier statement at line {tokens[current].Line}, (value: {tokens[current].Value}) (current: {current})");
            }
            else
            {
                throw new Exception($"Unexpected token {nextToken.Type} in identifier statement at line {nextToken.Line}, (value: {tokens[current].Value}) (current: {current})");
            }
        }
    }

    private ASTNode ParseFunctionArguments()
    {
        var functionArgumentsNode = new ASTNode { Type = "FunctionArguments" };
        Expect(TokenType.OpenParenthesis);

        while (current < tokens.Count && tokens[current].Type != TokenType.CloseParenthesis)
        {
            var token = tokens[current];

            if (token.Type == TokenType.StringLiteral)
            {
                functionArgumentsNode.Children.Add(ParseStringLiteral());
            }
            else
            {
                // error handling
                throw new Exception($"Unexpected token {token.Type} in function arguments at line {token.Line}, (value: {tokens[current].Value}) (current: {current})");
            }
        }

        Expect(TokenType.CloseParenthesis);

        return functionArgumentsNode;
    }

    private Token PeekNextToken()
    {
        int next = current + 1;
        return next < tokens.Count ? tokens[next] : null;
    }

    private ASTNode ParseImportStatement()
    {
        var importNode = new ASTNode { Type = "ImportStatement" };

        Expect(TokenType.Import);
        importNode.Children.Add(ParseIdentifier());

        Expect(TokenType.From);
        importNode.Children.Add(ParseStringLiteral());

        return importNode;
    }

    private void Expect(TokenType expectedType)
    {
        if (current < tokens.Count && tokens[current].Type == expectedType)
        {
            current++;
        }
        else
        {
            throw new Exception($"Expected token {expectedType} at line {tokens[current].Line}, (value: {tokens[current].Value}) (current: {current})");
        }
    }

    private ASTNode ParseStringLiteral()
    {
        var stringLiteralNode = new ASTNode { Type = "StringLiteral", Value = Consume(TokenType.StringLiteral) };
        return stringLiteralNode;
    }

    private string Consume(TokenType expectedType)
    {
        if (current < tokens.Count && tokens[current].Type == expectedType)
        {
            return tokens[current++].Value;
        }
        else
        {
            throw new Exception($"Expected token {expectedType}, but got {tokens[current].Type} at line {tokens[current].Line}, (value: {tokens[current].Value})  (current: {current})");
        }
    }

    private ASTNode ParseIdentifier()
    {
        var identifierNode = new ASTNode { Type = "Identifier", Value = Consume(TokenType.Identifier) };
        return identifierNode;
    }
}