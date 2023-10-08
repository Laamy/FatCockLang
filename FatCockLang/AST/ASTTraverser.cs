using System;

class ASTTraverser
{
    public static void Traverse(ASTNode node, string indent = "")
    {
        Console.WriteLine($"{indent}Type: {node.Type}, Value: {node.Value}");

        foreach (ASTNode child in node.Children)
        {
            Traverse(child, indent + "  ");
        }
    }
}