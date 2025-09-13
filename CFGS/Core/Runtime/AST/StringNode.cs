namespace CFGS.Core.Runtime.AST;

public class StringNode : Node
{
    public readonly string Value;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public StringNode(string val, int column, int line)
    {
        Value = val;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"\"{Value}\"";
}
