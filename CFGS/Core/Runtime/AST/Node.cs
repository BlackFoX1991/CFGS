namespace CFGS.Core.Runtime.AST;

public abstract class Node
{
    public abstract int Column { get; set; }
    public abstract int Line { get; set; }
    public abstract override string ToString();
}
