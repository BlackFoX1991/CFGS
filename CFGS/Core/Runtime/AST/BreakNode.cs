namespace CFGS.Core.Runtime.AST;

public class BreakNode : Node
{
    public override int Column { get; set; }
    public override int Line { get; set; }

    public BreakNode(int column, int line)
    {
        Column = column;
        Line = line;
    }

    public override string ToString() => "break";
}
