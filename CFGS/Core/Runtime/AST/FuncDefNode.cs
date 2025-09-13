namespace CFGS.Core.Runtime.AST;

public class FuncDefNode : Node
{
    public readonly string Name;
    public readonly List<string> Params;
    public readonly BlockNode Body;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public FuncDefNode(string name, List<string> parameters, BlockNode body, int column, int line)
    {
        Name = name;
        Params = parameters;
        Body = body;
        Column = column;
        Line = line;
    }

    public override string ToString() => $"func {Name}({string.Join(", ", Params)}) {Body.ToString()}";
}
