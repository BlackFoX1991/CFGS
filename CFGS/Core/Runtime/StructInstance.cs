namespace CFGS.Core.Runtime;

#pragma warning disable CS8602
public class StructInstance(string name, Dictionary<string, object?> fields) 
{
    public string Name { get; } = name;
    public Dictionary<string, object?> Fields { get; } = fields;

    public override string ToString()
    {
        var fieldsJson = string.Join(", ", Fields.Select(kv => $"\"{kv.Key}\": {kv.Value}"));
        return $"{{\n  \"type\": \"StructInstance\",\n  \"Fields\": {{ {fieldsJson} }}\n}}";
    }

}

public class BreakSignal : Exception { }

public class ContinueSignal : Exception { }