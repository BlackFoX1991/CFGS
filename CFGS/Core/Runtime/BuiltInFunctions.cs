using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFGS.Core.Runtime;


public static class BuiltInFunctions
{
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
                _ => throw new Exception("Invalid argument for 'len'")
            };
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
            return Console.ReadKey();
        },
        ["fopen"] = args =>
        {
            CheckArgs("fopen", args.Count, 2);
            var path = args[0]?.ToString() ?? throw new Exception("Invalid path");
            var mode = (FileMode)args[1]!;
            return new FileStream(path, mode);
        },
        ["fwrite"] = args =>
        {
            CheckArgs("fwrite", args.Count, 2);
            var fs = args[0] as FileStream ?? throw new Exception("Invalid FileStream");
            var content = args[1]?.ToString() ?? "";
            fs.Write(Encoding.UTF8.GetBytes(content));
            return 0;
        },
        ["fclose"] = args =>
        {
            CheckArgs("fclose", args.Count, 1);
            (args[0] as FileStream)?.Close();
            return 0;
        },
        ["pretty"] = args =>
        {
            CheckArgs("pretty", args.Count, 1);
            return args[0]?.ToString();
        }
    };

}
   

    



