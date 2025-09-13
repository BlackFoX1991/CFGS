namespace CFGS.Core.Runtime.AST;

public class MatchNode : Node
{
    public readonly Node Value;
    public readonly List<MatchCaseNode> Cases;
    public readonly Node? DefaultCase;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public MatchNode(Node value, List<MatchCaseNode> cases, Node? defaultCase, int column, int line)
    {
        Value = value;
        Cases = cases;
        DefaultCase = defaultCase;
        Column = column;
        Line = line;
    }

    public override string ToString()
    {
        var casesStr = string.Join("; ", Cases.Select(c => c.ToString()));
        var defaultStr = DefaultCase != null ? $" default => {DefaultCase.ToString()}" : "";
        return $"match ({Value.ToString()}) {{ {casesStr}{defaultStr} }}";
    }
}
