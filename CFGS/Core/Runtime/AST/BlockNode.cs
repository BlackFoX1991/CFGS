namespace CFGS.Core.Runtime.AST;

public class BlockNode : Node
{
    public readonly List<Node> Statements;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public BlockNode(List<Node> stmts, int column, int line)
    {
        Statements = stmts;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"{{{string.Join("; ", Statements.Select(s => s.ToString()))}}}";
}
