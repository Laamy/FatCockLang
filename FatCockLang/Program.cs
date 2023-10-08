using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        string code = @"import Console from ""ConsoleApi""

void main()
{
    Console.Write(""Hello, world!"")
    Console.Write(""Bye world..?"")
}";

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(code);

        Parser parser = new Parser(tokens);

        try
        {
            ASTNode program = parser.Parse();

            ASTTraverser.Traverse(program);

            // last but not least lets test our ast tree before writing the rest of the compiler
            Interpreter interpreter = new Interpreter();
            interpreter.Execute(program);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.ReadKey();
    }
}