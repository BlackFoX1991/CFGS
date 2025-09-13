namespace CFGS.Core.Runtime.AST;

public class EnumDefNode : Node
{
    public readonly string Name;
    // int? für optionale Werte; null bedeutet, dass der Wert automatisch hochgezählt wird
    public readonly List<KeyValuePair<string, int?>> Members;
    public override int Column { get; set; }
    public override int Line { get; set; }

    public EnumDefNode(string name, List<KeyValuePair<string, int?>> members, int column, int line)
    {
        Name = name;
        Members = members;
        Column = column;
        Line = line;
    }

    public override string ToString()
    {
        var membersStr = string.Join(", ", Members.Select(m => m.Value.HasValue ? $"{m.Key}={m.Value}" : m.Key));
        return $"enum {Name} {{ {membersStr} }}";
    }
}
