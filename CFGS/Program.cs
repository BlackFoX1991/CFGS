using CFGS.Core.Analytics;
using CFGS.Core.Runtime;

#pragma warning disable CS8602

namespace CFGS;

public static class Program
{
    public static void Main(string[] args)
    {
        
        var interpreter = new Interpreter();
        var mainScript = string.Empty;
        
        if (args.Length == 0)
        {
            ShowInfo();
            bool replb = true;
            while(replb)
            {
                var replc = Console.ReadLine();
                if(replc.Trim().StartsWith('$'))
                {
                    switch(replc.Substring(1).Trim())
                    {
                        case "help":
                            Console.WriteLine("You can run CFGS also with these commandset. They can be combined too.\n");
                            Console.WriteLine("-i <filepath>            : Loads a specific script");
                            Console.WriteLine("-w <filepath>            : Sets the path to the Main-Script");
                            Console.WriteLine("-r <code>                : Executes inline code");
                            Console.WriteLine("-r -f <Functionname>     : Calls a specific function from loaded script(s)");
                            Console.WriteLine("-r -m                    : Executes the Main-Script");
                            break;
                        case "exit":
                            replb = false;
                            break;

                        case "clear":
                            ShowInfo();
                            break;

                    }
                }
                else
                {
                    RunInlineCode(interpreter, replc);
                    Console.Write("> ");
                }
            }
        }
        else
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-w":
                        if (i + 1 >= args.Length) { Console.WriteLine("Missing path in -w"); return; }
                        mainScript = args[++i];
                        if (!File.Exists(mainScript))
                        {
                            Console.WriteLine($"Invalid script path '{mainScript}'.");
                            mainScript = string.Empty;
                        }
                        else
                        {
                            if (mainScript.Contains("\\"))
                                Directory.SetCurrentDirectory(mainScript.Substring(0, mainScript.LastIndexOf('\\')));
                        }

                        break;
                    case "-i":
                        if (i + 1 >= args.Length) { Console.WriteLine("Invalid filepath for -i <filepath>"); return; }
                        string initFile = args[++i];
                        LoadFileGlobals(interpreter, initFile);
                        break;

                    case "-r":
                        if (i + 1 >= args.Length) { Console.WriteLine("Invalid argument for -r"); return; }

                        if (args[i + 1] == "-f")
                        {
                            i++;
                            if (i + 1 >= args.Length) { Console.WriteLine("Invalid functions call for -f"); return; }
                            string funcCall = args[++i];
                            ExecuteInlineFunction(interpreter, funcCall);
                        }
                        else if (args[i + 1] == "-m")
                        {
                            i++;
                            if (!File.Exists(mainScript))
                            {
                                Console.WriteLine("Valid Main-Script has to be set with -w <filepath> first.");
                            }
                            else
                                RunFileFunction(interpreter, mainScript);
                        }
                        else
                        {
                            string code = args[++i];
                            RunInlineCode(interpreter, code);
                        }
                        break;

                    default:
                        Console.WriteLine($"Invalid argument: {args[i]}");
                        return;
                }
            }
        }

            
    }

    private static void ShowInfo()
    {
        Console.Clear();
        Console.WriteLine("CFGS 1.4b - Configuration Language");
        Console.WriteLine("Use $help, $exit or $clear");
        Console.Write("> ");
    }

    private static void LoadFileGlobals(Interpreter interpreter, string file)
    {
        if (!File.Exists(file)) { Console.WriteLine($"File does not exist : {file}"); return; }

        try
        {
            string code = File.ReadAllText(file);
            var tree = new Parser(new Lexer(code).GetTokens()).Parse();
            interpreter.VisitGlobals(tree);
            Console.WriteLine($"Loaded script '{file}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while loading script '{file}' : {ex.Message}");
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
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void ExecuteInlineFunction(Interpreter interpreter, string funcCall)
    {
        int paren = funcCall.IndexOf('(');
        if (paren == -1 || !funcCall.EndsWith(")")) { Console.WriteLine("Invalid use of functions."); return; }

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
            Console.WriteLine($"{name} : {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in fcall '{name}' : {ex.Message}");
        }
    }

    private static void RunFileFunction(Interpreter interpreter, string file)
    {
        if (!File.Exists(file)) { Console.WriteLine($"File does not exist : {file}"); return; }

        try
        {
            string code = File.ReadAllText(file);
            var tree = new Parser(new Lexer(code).GetTokens()).Parse();
            interpreter.Visit(tree); // nur Funktionen laden
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while executing script '{file}' : {ex.Message}");
        }
    }
}
