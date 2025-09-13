namespace CFGS.Core.Runtime.AST;

public class CharNode : Node
{
    public readonly char Value;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public CharNode(char val, int column, int line)
    {
        Value = val;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"'{Value}'";
}
