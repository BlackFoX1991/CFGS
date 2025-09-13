namespace CFGS.Core.Runtime.AST;

public class StructInstanceNode : Node
{
    public readonly string StructName;
    public readonly List<Node> FieldValues;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public StructInstanceNode(string structName, List<Node> fieldValues, int column, int line)
    {
        StructName = structName;
        FieldValues = fieldValues;
        Column = column;
        Line = line;
    }

    public override string ToString() =>
        $"{StructName} {{ {string.Join(", ", FieldValues.Select(f => f.ToString()))} }}";
}
