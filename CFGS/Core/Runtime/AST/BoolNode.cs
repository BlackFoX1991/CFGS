namespace CFGS.Core.Runtime.AST;

public class BoolNode : Node
{
    public readonly bool Value;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public BoolNode(bool val, int column, int line)
    {
        Value = val;
        Column = column;
        Line = line;
    }

    public override string ToString() => Value.ToString();
}
