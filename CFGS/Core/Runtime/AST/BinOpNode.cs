using CFGS.Core.Analytics;

namespace CFGS.Core.Runtime.AST;

public class BinOpNode : Node
{
    public readonly Node Left;
    public readonly Node Right;
    public readonly TokenType Op;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public BinOpNode(Node l, TokenType op, Node r, int column, int line)
    {
        Left = l;
        Op = op;
        Right = r;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"({Left.ToString()} {Op} {Right.ToString()})";
}
