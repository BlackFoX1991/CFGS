namespace CFGS.Core.Runtime.AST;

public class PrintNode : Node
{
    public readonly Node Value;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public PrintNode(Node val, int column, int line)
    {
        Value = val;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"print({Value.ToString()})";
}
