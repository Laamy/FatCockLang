using System;
using System.Collections.Generic;
using System.Data.Common;

class Interpreter
{
    private Environment globalEnvironment = new Environment(); // temp

    public void Execute(ASTNode programNode)
    {
        // setup global
        CreateGlobal();

        // traverse and execute
        TraverseAndExecute(programNode, globalEnvironment);

        // last but not least lets execute main (and error if it doesnt exist)
        Func<object[], object> main = globalEnvironment.GetFunction("main");
        if (main != null)
        {
            main(new object[0]);
        }
        else
        {
            throw new Exception("No main function found");
        }
    }

    private Environment LoadModule(string name)
    {
        // temp hardcoded for the concept
        Environment module = new Environment();

        module.DeclareFunction("Write", args =>
        {
            // temp function implementation
            if (args.Length == 1)
            {
                Console.WriteLine(args[0]);
            }
            else
            {
                throw new Exception($"{name}.Write() requires at least one argument");
            }

            return null;
        });

        return module;
    }

    private void CreateGlobal()
    {
        // TODO: Create global environment, like version, ect.
    }

    private void TraverseAndExecute(ASTNode node, Environment environment)
    {
        //Console.WriteLine($"Executing {node.Type}");
        switch(node.Type)
        {
            case "Program": // main program node so lets traverse all children
                foreach (ASTNode child in node.Children)
                {
                    TraverseAndExecute(child, environment);
                }
                break;

            case "CodeBlock":
                foreach (ASTNode child in node.Children)
                {
                    TraverseAndExecute(child, environment);
                }
                break;

            case "ImportStatement":
                {
                    string moduleName = node.Children[0].Value;
                    Environment module = LoadModule(moduleName);
                    environment.ImportModule(moduleName, module);
                }
                break;

            case "FunctionDeclaration":
                string functionName = node.Children[0].Value;
                Func<object[], object> function = args =>
                {
                    // create new envrionment for the function to handle (discard after ofc)
                    Environment funcEnvironment = new Environment(environment);

                    // bind arguments
                    for (int i = 0; i < args.Length; i++)
                    {
                        string arg = args[i] as string;

                        funcEnvironment.SetVariable($"arg{i}", args[i]);
                    }

                    // traverse function body
                    TraverseAndExecute(node.Children[1], funcEnvironment);

                    return null; // placeholder
                };
                environment.DeclareFunction(functionName, function);
                break;

            case "FunctionCall":
                string callName = node.Children[0].Value;
                Func<object[], object> call = environment.GetFunction(callName);

                // exists so lets call it
                if (callName != null && call != null)
                {
                    List<object> args = new List<object>();
                    foreach (ASTNode child in node.Children[1].Children)
                    {
                        args.Add(Evaluate(child, environment));
                    }
                    call(args.ToArray());
                }
                else
                {
                    // lets check our modules for the function
                    foreach (KeyValuePair<string, Environment> module in environment.GetModules())
                    {
                        // their trying to call it from this module so lets find & execute if exists
                        if (callName == module.Key)
                        {
                            // set callname to the next child
                            callName = node.Children[1].Value;

                            call = module.Value.GetFunction(callName);
                            if (call != null)
                            {
                                // exists so lets call it
                                List<object> args = new List<object>();
                                foreach (ASTNode child in node.Children[2].Children)
                                {
                                    args.Add(Evaluate(child, environment));
                                }
                                call(args.ToArray());
                                return;
                            }
                        }
                    }

                    // doesnt exist so error
                    throw new Exception($"Function {callName} does not exist");
                }
                break;
        }
    }

    private object Evaluate(ASTNode child, Environment environment)
    {
        switch (child.Type)
        {
            case "StringLiteral":
                return child.Value;

            case "Idenifier":
                string variableName = child.Value;
                return environment.GetVariable(variableName);

            default:
                throw new Exception($"Unknown node type: {child.Type}");
        }
    }
}