namespace CFGS.Core.Runtime.AST;

public class FuncCallNode : Node
{
    public readonly string Name;
    public readonly List<Node> Args;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public FuncCallNode(string name, List<Node> args, int column, int line)
    {
        Name = name;
        Args = args;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"{Name}({string.Join(", ", Args.Select(a => a.ToString()))})";
}
