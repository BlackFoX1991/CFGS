using CFGS.Core.Analytics;
using CFGS.Core.Runtime;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace CFGS;

public static class Program
{
    public static void Main(string[] args)
    {
        var interpreter = new Interpreter();
            
        if (args.Length > 0)
        {
            foreach (var file in args)
            {
                if (!File.Exists(file))
                {
                    Console.WriteLine($"Datei nicht gefunden: {file}");
                    continue;
                }

                try
                {
                    string code = File.ReadAllText(file);
                    RunCode(interpreter, code);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Ausführen von {file}: {ex.Message}");
                }
            }
        }
        else
            RunRepl(interpreter);
            
            
    }

    private static void RunRepl(Interpreter interpreter)
    {
        Console.WriteLine("Configus ( Version 1.3a ) - REPL");
        var inputLines = new List<string>();

        while (true)
        {
            Console.Write(inputLines.Count == 0 ? "> " : ". ");
            string? line = Console.ReadLine();
            if (line == null) break;
            if (line.Trim().ToLower() == "exit") break;

            inputLines.Add(line);

            if (IsInputComplete(inputLines))
            {
                string code = string.Join("\n", inputLines);
                inputLines.Clear();

                try
                {
                    RunCode(interpreter,code);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler: {ex.Message}");
                }
            }
        }
    }

    private static void RunCode(Interpreter interpreter, string code)
    {
        var lexer = new Lexer(code);
        var tokens = lexer.GetTokens();

        var parser = new Parser(tokens);
        var tree = parser.Parse();
        //Console.WriteLine(tree.ToString());
        interpreter.Visit(tree);
    }
   


    private static bool IsInputComplete(List<string> lines)
    {
        int open = 0;
        foreach (var line in lines)
        {
            foreach (var c in line)
            {
                if (c == '{' || c == '[') open++;
                if (c == '}' || c == ']') open--;
            }
        }

        return open <= 0;
    }
}