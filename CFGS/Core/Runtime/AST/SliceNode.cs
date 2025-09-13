namespace CFGS.Core.Runtime.AST;

public class SliceNode : Node
{
    public readonly Node Target;
    public readonly Node? Start;
    public readonly Node? End;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public SliceNode(Node target, Node start, Node end, int column, int line)
    {
        Target = target;
        Start = start;
        End = end;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"{Target.ToString()}[{Start?.ToString()}:{End?.ToString()}]";
}
