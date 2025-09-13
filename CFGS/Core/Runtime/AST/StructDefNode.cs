namespace CFGS.Core.Runtime.AST;

public class StructDefNode : Node
{
    public readonly string Name;
    public readonly List<string> Fields;
    public readonly List<Node>? Methods;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public StructDefNode(string name, List<string> fields, List<Node>? methods, int column, int line)
    {
        Name = name;
        Fields = fields;
        Methods = methods;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"struct {Name} {{ {string.Join(", ", Fields)} }}";
}
