namespace CFGS.Core.Runtime.AST;

public class ArrayDeleteNode : Node
{
    public readonly Node Array;
    public readonly Node? Index;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public ArrayDeleteNode(Node array, Node index, int column, int line)
    {
        Array = array;
        Index = index;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"delete {Array}[{Index}]";
}
