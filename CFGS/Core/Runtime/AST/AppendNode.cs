namespace CFGS.Core.Runtime.AST;

public class AppendNode : Node
{
    public readonly Node Array;
    public readonly Node Value;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public AppendNode(Node array, Node value, int column, int line)
    {
        Array = array;
        Value = value;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"{Array.ToString()}[] = {Value.ToString()}";
}
