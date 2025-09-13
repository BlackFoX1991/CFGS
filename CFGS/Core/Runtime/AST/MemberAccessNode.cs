namespace CFGS.Core.Runtime.AST;

public class MemberAccessNode : Node
{
    public readonly Node ObjectNode;
    public readonly string MemberName;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public MemberAccessNode(Node objNode, string memberName, int column, int line)
    {
        ObjectNode = objNode;
        MemberName = memberName;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"{ObjectNode.ToString()}.{MemberName}";
}
