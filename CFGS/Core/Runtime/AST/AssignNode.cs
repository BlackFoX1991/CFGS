namespace CFGS.Core.Runtime.AST;

public class AssignNode : Node
{
    public readonly Node Target;
    public readonly Node Value;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public AssignNode(Node target, Node value, int column, int line)
    {
        Target = target;
        Value = value;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"({Target.ToString()} = {Value.ToString()})";
}
