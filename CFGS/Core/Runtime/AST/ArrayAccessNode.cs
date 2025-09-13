namespace CFGS.Core.Runtime.AST;

public class ArrayAccessNode : Node
{
    public readonly Node Array;
    public readonly Node? Index; // ← nullable
    public override int Column { get; set; }
    public override int Line { get; set; }

    public ArrayAccessNode(Node array, Node? index, int column, int line)
    {
        Array = array;
        Index = index;
        Column = column;
        Line = line;
    }

    public override string ToString() => Index == null ? $"{Array}[]" : $"{Array}[{Index}]";
}
