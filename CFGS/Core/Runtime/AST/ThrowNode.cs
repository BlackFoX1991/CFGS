namespace CFGS.Core.Runtime.AST;

public class ThrowNode : Node
{
    public readonly Node Value;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public ThrowNode(Node value, int column, int line)
    {
        Value = value;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"throw {Value.ToString()}";
}
