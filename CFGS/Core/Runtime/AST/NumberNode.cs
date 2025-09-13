namespace CFGS.Core.Runtime.AST;

public class NumberNode : Node
{
    public readonly double Value;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public NumberNode(double val, int column, int line)
    {
        Value = val;
        Column = column;
        Line = line;
    }

    public override string ToString() => Value.ToString();
}
