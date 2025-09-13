using System.Text;
using System.Text.Json;

namespace CFGS.Core.Analytics;
#pragma warning disable CS8602
#pragma warning disable CS8604

public class EnumDef
{
    public readonly string Name;
    public readonly List<KeyValuePair<string, int?>> Members;

    public EnumDef(string name, List<KeyValuePair<string, int?>> members)
    {
        Name = name;
        Members = members;
    }

    public override string ToString()
    {
        var membersJson = Members
            .Select(m => $"\"{m.Key}\": {m.Value?.ToString() ?? "null"}")
            .Aggregate((current, next) => current + ", " + next);

        return "{ " +
               $"\"type\": \"Enum\", " +
               $"\"name\": \"{Name}\", " +
               $"\"members\": {{ {membersJson} }}" +
               " }";
    }
}




