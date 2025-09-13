namespace CFGS.Core.Runtime.AST;

public class ContinueNode : Node
{
    public override int Column { get; set; }
    public override int Line { get; set; }

    public ContinueNode(int column, int line)
    {
        Column = column;
        Line = line;
    }

    public override string ToString() => "continue";
}
