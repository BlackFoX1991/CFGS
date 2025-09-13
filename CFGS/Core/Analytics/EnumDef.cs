using System.Text;
using System.Text.Json;

namespace CFGS.Core.Analytics;
#pragma warning disable CS8602
#pragma warning disable CS8604

public class EnumDef
{
    public string Name { get; }
    public Dictionary<string, int> Members { get; } = new();

    public EnumDef(string name, List<KeyValuePair<string, int?>> members)
    {
        Name = name;
        int counter = 0;
        var usedValues = new HashSet<int>();

        foreach (var kv in members)
        {
            int value = kv.Value ?? counter;

            if (usedValues.Contains(value))
                throw new Exception($"Duplicate enum value {value} in enum {name}.");

            Members[kv.Key] = value;
            usedValues.Add(value);

            counter = value + 1;
        }
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(new
        {
            type = "Enum",
            name = Name,
            members = Members
        });
    }
}



