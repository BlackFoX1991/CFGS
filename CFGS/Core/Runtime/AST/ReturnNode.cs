namespace CFGS.Core.Runtime.AST;

public class ReturnNode : Node
{
    public readonly Node Value;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public ReturnNode(Node value, int column, int line)
    {
        Value = value;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"return {Value.ToString()}";
}
