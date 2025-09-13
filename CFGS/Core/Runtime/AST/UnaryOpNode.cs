using CFGS.Core.Analytics;

namespace CFGS.Core.Runtime.AST;

public class UnaryOpNode : Node
{
    public TokenType Op { get; }
    public Node Node { get; }
    public bool IsPrefix { get; }

    public override int Column { get; set; }
    public override int Line { get; set; }

    public UnaryOpNode(TokenType op, Node node, int column, int line, bool isPrefix = true)
    {
        Op = op;
        Node = node;
        IsPrefix = isPrefix;

        Column = column;
        Line = line;
    }
    public override string ToString() => $"({Op}{Node.ToString()})";
}
