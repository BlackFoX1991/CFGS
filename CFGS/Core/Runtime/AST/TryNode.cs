namespace CFGS.Core.Runtime.AST;

public class TryNode : Node
{
    public readonly Node TryBlock;
    public readonly Node? CatchVar;
    public readonly Node? CatchBlock;
    public readonly Node? FinallyBlock;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public TryNode(Node tryBlock, Node? catchVar, Node? catchBlock, Node? finallyBlock, int column, int line)
    {
        TryBlock = tryBlock;
        CatchVar = catchVar;
        CatchBlock = catchBlock;
        FinallyBlock = finallyBlock;
        Column = column;
        Line = line;
    }

    public override string ToString()
    {
        var s = $"try {TryBlock.ToString()}";
        if (CatchVar != null || CatchBlock != null)
            s += $" catch ({CatchVar?.ToString() ?? ""}) {CatchBlock?.ToString() ?? ""}";
        if (FinallyBlock != null)
            s += $" finally {FinallyBlock.ToString()}";
        return s;
    }
}
