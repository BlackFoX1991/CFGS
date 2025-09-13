namespace CFGS.Core.Runtime.AST;

public class VarNode : Node
{
    public readonly string Name;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public VarNode(string name, int column, int line)
    {
        Name = name;
        Column = column;
        Line = line;
    }

    public override string ToString() => Name;
}
