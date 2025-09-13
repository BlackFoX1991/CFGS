namespace CFGS.Core.Runtime.AST;

public class IfNode : Node
{
    public readonly Node Condition;
    public readonly Node Body;
    public readonly Node? ElseBody;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public IfNode(Node cond, Node thenBody, int column, int line, Node? elseBody = null)
    {
        Condition = cond;
        Body = thenBody;
        ElseBody = elseBody;
        Column = column;
        Line = line;
    }

    public override string ToString() =>
        ElseBody == null
            ? $"if ({Condition.ToString()}) {Body.ToString()}"
            : $"if ({Condition.ToString()}) {Body.ToString()} else {ElseBody.ToString()}";
}
