namespace CFGS.Core.Runtime.AST;

public class MatchCaseNode : Node
{
    public readonly List<Node> Values;
    public readonly Node Body;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public MatchCaseNode(List<Node> values, Node body, int column, int line)
    {
        Values = values;
        Body = body;
        Column = column;
        Line = line;
    }

    public override string ToString() =>
        $"case [{string.Join(", ", Values.Select(v => v.ToString()))}] => {Body.ToString()}";
}
