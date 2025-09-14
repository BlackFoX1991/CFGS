using CFGS.Core.Analytics;
using CFGS.Core.Runtime;

namespace CFGS;

public static class Program
{
    public static string _version = "1.4.3b";
    public static void Main(string[] args)
    {
        var interpreter = new Interpreter();
        var mainScript = string.Empty;

        if (args.Length == 0)
        {
            RunRepl(interpreter);
        }
        else
        {
            RunWithArguments(interpreter, args, ref mainScript);
        }
    }

    // =====================
    // REPL
    // =====================
    private static void RunRepl(Interpreter interpreter)
    {
        ShowBanner();

        bool isRunning = true;
        while (isRunning)
        {
            string codeBlock = ReadMultilineInput();
            if (string.IsNullOrWhiteSpace(codeBlock))
                continue;

            if (codeBlock.StartsWith('$'))
            {
                isRunning = HandleReplCommand(interpreter, codeBlock);
            }
            else
            {
                RunInlineCode(interpreter, codeBlock);
            }
        }
    }

    private static string ReadMultilineInput()
    {
        var buffer = new List<string>();
        string? line;
        string prompt = "> ";

        do
        {
            Console.Write(prompt);
            line = Console.ReadLine();

            if (line == null)
                break;

            buffer.Add(line);

            prompt = IsInputComplete(string.Join(Environment.NewLine, buffer)) ? "> " : "... ";

        } while (prompt == "... ");

        return string.Join(Environment.NewLine, buffer).Trim();
    }

    private static bool IsInputComplete(string input)
    {
        int paren = 0, brace = 0;
        bool inString = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (c == '"' && (i == 0 || input[i - 1] != '\\'))
            {
                inString = !inString; // toggle string mode
            }
            else if (!inString)
            {
                if (c == '(') paren++;
                if (c == ')') paren--;
                if (c == '{') brace++;
                if (c == '}') brace--;
            }
        }

        return !inString && paren <= 0 && brace <= 0;
    }

    private static bool HandleReplCommand(Interpreter interpreter, string command)
    {
        switch (command[1..].Trim())
        {
            case "help":
                Console.WriteLine("Available REPL commands:\n");
                Console.WriteLine("-i <filepath>            : Loads a specific script");
                Console.WriteLine("-w <filepath>            : Sets the path to the Main-Script");
                Console.WriteLine("-r <code>                : Executes inline code");
                Console.WriteLine("-r -f <Functionname>     : Calls a specific function from loaded script(s)");
                Console.WriteLine("-r -m                    : Executes the Main-Script");
                Console.WriteLine("\nSpecial REPL commands:");
                Console.WriteLine("$help, $exit, $clear");
                return true;

            case "exit":
                return false;

            case "clear":
                Console.Clear();
                ShowBanner();
                return true;

            default:
                Console.WriteLine($"[CFGS Error] Unknown command: {command}");
                return true;
        }
    }

    private static void ShowBanner()
    {
        Console.WriteLine($"CFGS {_version} - Configuration Language");
        Console.WriteLine("Type $help for commands, $exit to quit.\n");
    }

    // =====================
    // ARGUMENT HANDLING
    // =====================
    private static void RunWithArguments(Interpreter interpreter, string[] args, ref string mainScript)
    {
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-w":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("[CFGS Error] Missing path in -w");
                        return;
                    }

                    mainScript = args[++i];
                    if (!File.Exists(mainScript))
                    {
                        Console.WriteLine($"[CFGS Error] Invalid script path '{mainScript}'.");
                        mainScript = string.Empty;
                    }
                    else
                    {
                        var dir = Path.GetDirectoryName(mainScript);
                        if (!string.IsNullOrEmpty(dir))
                            Directory.SetCurrentDirectory(dir);
                    }
                    break;

                case "-i":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("[CFGS Error] Invalid filepath for -i <filepath>");
                        return;
                    }
                    LoadFileGlobals(interpreter, args[++i]);
                    break;

                case "-r":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("[CFGS Error] Invalid argument for -r");
                        return;
                    }

                    if (args[i + 1] == "-f")
                    {
                        i++;
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("[CFGS Error] Invalid function call for -f");
                            return;
                        }
                        ExecuteInlineFunction(interpreter, args[++i]);
                    }
                    else if (args[i + 1] == "-m")
                    {
                        i++;
                        if (!File.Exists(mainScript))
                        {
                            Console.WriteLine("[CFGS Error] Valid Main-Script must be set with -w <filepath> first.");
                        }
                        else
                        {
                            RunFileFunction(interpreter, mainScript);
                        }
                    }
                    else
                    {
                        RunInlineCode(interpreter, args[++i]);
                    }
                    break;

                default:
                    Console.WriteLine($"[CFGS Error] Invalid argument: {args[i]}");
                    return;
            }
        }
    }

    // =====================
    // HELPERS
    // =====================
    private static void LoadFileGlobals(Interpreter interpreter, string file)
    {
        if (!File.Exists(file))
        {
            Console.WriteLine($"[CFGS Error] File does not exist: {file}");
            return;
        }

        try
        {
            string code = File.ReadAllText(file);
            var tree = new Parser(new Lexer(code).GetTokens()).Parse();
            interpreter.VisitGlobals(tree);
            Console.WriteLine($"[CFGS] Loaded script '{file}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CFGS Error] While loading '{file}': {ex.Message}");
        }
    }

    private static void RunInlineCode(Interpreter interpreter, string code)
    {
        try
        {
            var tree = new Parser(new Lexer(code).GetTokens()).Parse();
            interpreter.Visit(tree);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CFGS Error] {ex.Message}");
        }
    }

    private static void ExecuteInlineFunction(Interpreter interpreter, string funcCall)
    {
        int paren = funcCall.IndexOf('(');
        if (paren == -1 || !funcCall.EndsWith(")"))
        {
            Console.WriteLine("[CFGS Error] Invalid function call syntax.");
            return;
        }

        string name = funcCall[..paren].Trim();
        string argsStr = funcCall[(paren + 1)..^1];

        var argsList = new List<object?>();
        if (!string.IsNullOrWhiteSpace(argsStr))
        {
            foreach (var a in argsStr.Split(',', StringSplitOptions.TrimEntries))
            {
                if (int.TryParse(a, out var intVal)) argsList.Add(intVal);
                else if (double.TryParse(a, out var dblVal)) argsList.Add(dblVal);
                else if (bool.TryParse(a, out var boolVal)) argsList.Add(boolVal);
                else argsList.Add(a.Trim('"'));
            }
        }

        try
        {
            var result = interpreter.CallFunctionByName(name, argsList);
            Console.WriteLine($"{name} => {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CFGS Error] Function '{name}': {ex.Message}");
        }
    }

    private static void RunFileFunction(Interpreter interpreter, string file)
    {
        if (!File.Exists(file))
        {
            Console.WriteLine($"[CFGS Error] File does not exist: {file}");
            return;
        }

        try
        {
            string code = File.ReadAllText(file);
            var tree = new Parser(new Lexer(code).GetTokens()).Parse();
            interpreter.Visit(tree); // Load and run main
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CFGS Error] While executing '{file}': {ex.Message}");
        }
    }
}
