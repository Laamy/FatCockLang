using System.Collections.Generic;

class ASTNode
{
    public string Type { get; set; }
    public string Value { get; set; }
    public List<ASTNode> Children { get; set; } = new List<ASTNode>();
}