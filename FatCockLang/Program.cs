using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        string code = @"import sad from ""ConsoleApi"";

void foo() 
{
    Console.Write(""Hello, world!"")
}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(code);

        Parser parser = new Parser(tokens);
        
        try
        {
            ASTNode program = parser.Parse();

            ASTTraverser.Traverse(program);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.ReadKey();
    }
}