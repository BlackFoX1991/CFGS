namespace CFGS.Core.Runtime.AST;

public class ArrayNode : Node
{
    public readonly List<Node> Elements;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public ArrayNode(List<Node> elements, int column, int line)
    {
        Elements = elements;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"[{string.Join(", ", Elements.Select(e => e.ToString()))}]";
}
