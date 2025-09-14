using CFGS.Core.Runtime.AST;
using CFGS.Core.Runtime.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGS.Core.Analytics
{
    public static class ValueFormat
    {
        public static string FormatValue(object? value)
        {
            if (value == null) return "null";

            // StringNode → JSON-escaped
            if (value is StringNode sNode)
                return $"{EscapeJson(sNode.Value)}";

            // Plain string → JSON-escaped
            if (value is string str)
                return $"{EscapeJson(str)}";

            // Just output chars as they come
            if (value is char cr)
                return $"{(cr)}";

            // ArrayNode → JSON Array
            if (value is ArrayNode arrNode)
                return "[" + string.Join(", ", arrNode.Elements.Select(e => FormatValue(e))) + "]";

            // List<object?> → JSON Array
            if (value is List<object?> list)
                return "[" + string.Join(", ", list.Select(FormatValue)) + "]";

            // Dictionary → JSON Object
            if (value is Dictionary<string, object?> dict)
            {
                var entries = dict.Select(kv => $"\"{EscapeJson(kv.Key)}\": {FormatValue(kv.Value)}");
                return "{" + string.Join(", ", entries) + "}";
            }

            // StructInstance → JSON Object
            if (value is StructInstance si)
            {
                var entries = si.Fields.Select(kv => $"\"{EscapeJson(kv.Key)}\": {FormatValue(kv.Value)}");
                return "{" + string.Join(", ", entries) + "}";
            }

            // Enum → JSON string
            if (value is EnumDef ev)
                return $"\"{EscapeJson(ev.ToString())}\"";

            // Node allgemein → fallback auf ToString()
            if (value is Node node)
                return $"\"{EscapeJson(node.ToString())}\"";

            // Numerische Werte → unquoted
            if (value is int or long or float or double or decimal or bool)
                return value.ToString()?.ToLower() ?? "null";

            // Fallback → JSON string
            return $"\"{EscapeJson(value.ToString() ?? "")}\"";
        }

        // JSON-escaping inklusive \r
        public static string EscapeJson(string s)
        {
            return s.Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\b", "\\b")
                    .Replace("\f", "\\f")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r") // Carriage Return escaped
                    .Replace("\t", "\\t");
        }
    }
}
