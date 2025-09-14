using CFGS.Core.Analytics;
using System.Text;

namespace CFGS.Core.Runtime;

public static class BuiltInFunctions
{

    public static int CallCol = 0;
    public static int CallLine = 0;
    public static void CheckArgs(string name, int currentCount, int expected)
    {
        if (currentCount != expected)
            throw new Exception($"Invalid argument count for '{name}()': expected {expected}, got {currentCount}");
    }

    public static Dictionary<string, Func<List<object?>, object?>> builtinfuncs = new Dictionary<string, Func<List<object?>, object?>>(StringComparer.OrdinalIgnoreCase)
    {
        ["len"] = args =>
        {
            CheckArgs("len", args.Count, 1);
            var arg = args[0];
            return arg switch
            {
                List<object?> list => list.Count,
                string s => s.Length,
                FileStream f => f.Length,
                _ => throw new Exception($"Invalid argument for 'len' at line {CallLine}, column {CallCol}.")
            };
        },
        ["isarray"] = args =>
        {
            CheckArgs("len", args.Count, 1);
            var arg = args[0];
            return arg is List<object?>;
        },
        ["fpos"] = args =>
        {
            CheckArgs("fpos", args.Count, 1);
            var fs = args[0] as FileStream ?? throw new Exception($"Invalid FileStream at line {CallLine}, column {CallCol}.");
            return fs.Position;
        },
        ["toint32"] = args =>
        {
            CheckArgs("toint32", args.Count, 1);
            return Convert.ToInt32(args[0]);
        },
        ["toint64"] = args =>
        {
            CheckArgs("toint64", args.Count, 1);
            return Convert.ToInt64(args[0]);
        },
        ["chr"] = args =>
        {
            CheckArgs("chr", args.Count, 1);
            return Convert.ToChar(args[0]);
        },
        ["str"] = args =>
        {
            CheckArgs("chr", args.Count, 1);
            return ValueFormat.FormatValue(args[0]);
        },
        ["todbl"] = args =>
        {
            CheckArgs("todbl", args.Count, 1);
            return Convert.ToDouble(args[0]);
        },
        ["getl"] = args =>
        {
            CheckArgs("getl", args.Count, 0);
            return Console.ReadLine();
        },
        ["getc"] = args =>
        {
            CheckArgs("getc", args.Count, 0);
            return Console.Read();
        },
        ["getk"] = args =>
        {
            CheckArgs("getk", args.Count, 0);
            return Console.ReadKey().Key.ToString();
        },
        ["fopen"] = args =>
        {
            CheckArgs("fopen", args.Count, 3);
            var path = args[0]?.ToString() ?? throw new Exception($"Invalid path at line {CallLine}, column {CallCol}.");
            var mode = (FileMode)args[1]!;
            var acc = (FileAccess)args[2]!;
            return new FileStream(path, mode, acc);
        },
        ["fwrite"] = args =>
        {
            CheckArgs("fwrite", args.Count, 2);
            var fs = args[0] as FileStream ?? throw new Exception($"Invalid FileStream at line {CallLine}, column {CallCol}.");
            var content = args[1]?.ToString() ?? "";
            fs.Write(Encoding.UTF8.GetBytes(content));
            return 0;
        },
        ["fread"] = args =>
        {
            CheckArgs("fread", args.Count, 1);
            var fs = args[0] as FileStream ?? throw new Exception($"Invalid FileStream at line {CallLine}, column {CallCol}.");
            return fs.ReadByte();
        },
        ["fclose"] = args =>
        {
            CheckArgs("fclose", args.Count, 1);
            (args[0] as FileStream)?.Close();
            return 0;
        },
        ["fexist"] = args =>
        {
            CheckArgs("fexist", args.Count, 1);
            return File.Exists((args[0] as StringNode)?.Value ?? "");
        }
    };

}
