using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string code = File.ReadAllText("script.fc");

        // setup system modules first
        GlobalModules.Init();

        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(code);

        Parser parser = new Parser(tokens);

        try
        {
            ASTNode program = parser.Parse();

            ASTTraverser.Traverse(program);

            Console.WriteLine("Executing program...\r\n");

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