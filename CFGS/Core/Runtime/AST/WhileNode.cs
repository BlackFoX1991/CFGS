namespace CFGS.Core.Runtime.AST;

public class WhileNode : Node
{
    public readonly Node Condition;
    public readonly Node Body;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public WhileNode(Node cond, Node body, int column, int line)
    {
        Condition = cond;
        Body = body;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"while ({Condition.ToString()}) {Body.ToString()}";
}
