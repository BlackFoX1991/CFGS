using CFGS.Core.Analytics;
using Microsoft.VisualBasic;

namespace CFGS.Core.Runtime;

#pragma warning disable CS8602
public class Interpreter
{
    private readonly List<Dictionary<string, object?>> _scopes = new();
    private readonly Dictionary<string, FuncDefNode> _functions = new();
    private readonly Dictionary<string, StructDefNode> _structs = new();
    private readonly HashSet<string> _importedFiles = new();


    public Interpreter()
    {
        _scopes.Add(new Dictionary<string, object?>()); // global scope
    }

    private void PushScope() => _scopes.Add(new Dictionary<string, object?>());
    private void PopScope() => _scopes.RemoveAt(_scopes.Count - 1);

    private object? GetVariable(string name)
    {
        for (int i = _scopes.Count - 1; i >= 0; i--)
            if (_scopes[i].ContainsKey(name))
                return _scopes[i][name];
        throw new Exception($"Variable {name} not defined");
    }

    private void SetVariable(string name, object? value)
    {
        for (int i = _scopes.Count - 1; i >= 0; i--)
        {
            if (_scopes[i].ContainsKey(name))
            {
                _scopes[i][name] = value;
                return;
            }
        }

        // not found: define in current scope
        _scopes[^1][name] = value;
    }

    public void Visit(Node node)
    {
        switch (node)
        {
            case BlockNode b:
                foreach (var stmt in b.Statements) Visit(stmt);
                break;

            case PrintNode p:
                Console.WriteLine(Eval(p.Value));
                break;

            case AssignNode a:
            {
                var val = Eval(a.Value);

                if (a.Target is VarNode v)
                {
                    SetVariable(v.Name, val);
                }
                else if (a.Target is ArrayAccessNode aa)
                {
                    var arrayObj = Eval(aa.Array);
                    if (arrayObj is not List<object?> list)
                        throw new Exception("Trying to index a non-array");

                    var idxObj = Eval(aa.Index);
                    int idx = Convert.ToInt32(idxObj);

                    if (idx < 0 || idx >= list.Count)
                        throw new Exception("Array index out of bounds");

                    list[idx] = val;
                }
                else if (a.Target is MemberAccessNode ma)
                {
                    var obj = Eval(ma.ObjectNode);
                    if (obj is StructInstance si)
                    {
                        if (!si.Fields.ContainsKey(ma.MemberName))
                            throw new Exception($"Struct {si.Name} has no field {ma.MemberName}");
                        si.Fields[ma.MemberName] = val;
                    }
                    else
                    {
                        throw new Exception("Invalid assignment target (not a struct instance)");
                    }
                }
                else
                {
                    throw new Exception("Invalid assignment target");
                }

                break;
            }
            case TryNode t:
                try
                {
                    Visit(t.TryBlock);
                }
                catch (Exception ex)
                {
                    if (t.CatchBlock != null)
                    {
                        if (t.CatchVar is VarNode vn)
                            SetVariable(vn.Name, ex); // Fehler in Variable speichern

                        Visit(t.CatchBlock);
                    }
                    else
                    {
                        throw; // kein Catch-Block → weiterwerfen
                    }
                }
                finally
                {
                    if (t.FinallyBlock != null)
                        Visit(t.FinallyBlock);
                }
                break;
            case ThrowNode thr:
                var tval = Eval(thr.Value);
                throw new Exception(Convert.ToString(tval)); // wirft den Fehler an den Catch-Block weiter
            case MatchNode m:
                var matchVal = Eval(m.Value);
                bool matched = false;

                foreach (var c in m.Cases)
                {
                    foreach (var v in c.Values)
                    {
                        if (Eval(v).Equals(matchVal))
                        {
                            Visit(c.Body);
                            matched = true;
                            break;
                        }
                    }
                    if (matched) break;
                }

                if (!matched && m.DefaultCase != null)
                    Visit(m.DefaultCase);

                break;

            case IfNode i:
                if (Convert.ToBoolean(Eval(i.Condition)))
                {
                    Visit(i.Body);
                }
                else if (i.ElseBody != null)
                {
                    Visit(i.ElseBody);
                }
                break;

            case WhileNode w:
                while (Convert.ToBoolean(Eval(w.Condition))) 
                {
                    try
                    {
                        Visit(w.Body);
                    }
                    catch (BreakSignal)
                    {
                        break; // Schleife verlassen
                    }
                    catch (ContinueSignal)
                    {
                        continue; // zum nächsten Durchlauf springen
                    }
                }
                break;
            case BreakNode b:
                throw new BreakSignal();
            case ContinueNode c:
                throw new ContinueSignal();

            case ImportNode imp:
                string fullPath = Path.GetFullPath(imp.FileName);
                if (_importedFiles.Contains(fullPath)) break;
                if (!File.Exists(fullPath)) throw new Exception($"Import file not found: {fullPath}");
                _importedFiles.Add(fullPath);
                string code = File.ReadAllText(fullPath);
                var lexer = new Lexer(code);
                var tokens = lexer.GetTokens();
                var parser = new Parser(tokens);
                var tree = parser.Parse();
                Visit(tree);
                break;

            case FuncDefNode f:
                _functions[f.Name] = f;
                break;

            case StructDefNode s:
                _structs[s.Name] = s;
                break;

            case FuncCallNode call:
                Eval(call); // function call as statement
                break;

            case ReturnNode r:
                throw new cException.ReturnException(Eval(r.Value));

            default:
                Eval(node);
                break;
        }
    }

    private object? CallFunction(FuncDefNode f, List<object?> argVals)
    {
        PushScope();
        for (int i = 0; i < f.Params.Count; i++)
        {
            var pname = f.Params[i];
            var pval = i < argVals.Count ? argVals[i] : null;
            SetVariable(pname, pval);
        }

        try
        {
            Visit(f.Body);
            return null;
        }
        catch (cException.ReturnException re)
        {
            return re.Value;
        }
        finally
        {
            PopScope();
        }
    }

    private object? Eval(Node node)
    {
        switch (node)
        {
            case NumberNode n: return n.Value;
            case StringNode s: return s.Value;
            case CharNode c: return c.Value;
            case BoolNode b: return b.Value;
            case VarNode v: return GetVariable(v.Name);

            case ArrayNode a:
                var result = new List<object?>();
                foreach (var element in a.Elements)
                    result.Add(Eval(element));
                return result;

            case ArrayAccessNode ac:
            {
                var arrayObj = Eval(ac.Array);
                switch (arrayObj)
                {
                    case List<object?> list:
                    {
                        var idxObj = Eval(ac.Index);
                        int idx = Convert.ToInt32(idxObj);

                        if (idx < 0 || idx >= list.Count)
                            throw new Exception("Array index out of bounds");

                        return list[idx];
                    }
                    case string stx:
                    {
                        var idxStr = Eval(ac.Index);
                        int idx = Convert.ToInt32(idxStr);

                        if (idx < 0 || idx >= stx.Length)
                            throw new Exception("String index out of bounds");

                        return Convert.ToChar(stx[idx]);
                    }
                    default:
                        throw new Exception("Trying to index a non-array");
                }
            }

            case StructInstanceNode si:
                if (!_structs.TryGetValue(si.StructName, out var sdef))
                    throw new Exception($"Struct not defined: {si.StructName}");

                var instance = new Dictionary<string, object?>();
                for (int i = 0; i < sdef.Fields.Count; i++)
                {
                    object? xval = i < si.FieldValues.Count ? Eval(si.FieldValues[i]) : null;
                    instance[sdef.Fields[i]] = xval;
                }

                return new StructInstance(si.StructName, instance);

            case MemberAccessNode ma:
            {
                var obj = Eval(ma.ObjectNode);
                if (obj is StructInstance sInst)
                {
                    if (!sInst.Fields.TryGetValue(ma.MemberName, out var eval))
                        throw new Exception($"Struct {sInst.Name} has no field {ma.MemberName}");
                    return eval;
                }
                throw new Exception($"Member access on non-struct instance at line {ma.Line}, column {ma.Column}.");
            }

            case FuncCallNode fc:
                if (fc.Name == "len")
                {
                    if (fc.Args.Count != 1) throw new Exception($"Invalid argument for {fc.Name}().");
                    var arg = Eval(fc.Args[0]);
                    return arg switch
                    {
                        List<object?> list => list.Count,
                        string st => st.Length,
                        _ => throw new Exception($"Invalid argument for {fc.Name}().")
                    };
                }
                else if(fc.Name == "toint32")
                {
                    if (fc.Args.Count != 1) throw new Exception($"Invalid argument for {fc.Name}().");
                    var arg = Eval(fc.Args[0]);
                    return Convert.ToInt32(arg);
                }
                else if (fc.Name == "toint64")
                {
                    if (fc.Args.Count != 1) throw new Exception($"Invalid argument for {fc.Name}().");
                    var arg = Eval(fc.Args[0]);
                    return Convert.ToInt64(arg);
                }
                else if (fc.Name == "todbl")
                {
                    if (fc.Args.Count != 1) throw new Exception($"Invalid argument for {fc.Name}().");
                    var arg = Eval(fc.Args[0]);
                    return Convert.ToDouble(arg);
                }
                else if(fc.Name == "getl")
                {
                    if (fc.Args.Count > 0) throw new Exception($"Invalid argument for {fc.Name}().");
                    return Console.ReadLine();
                }
                else if (fc.Name == "getc")
                {
                    if (fc.Args.Count > 0) throw new Exception($"Invalid argument for {fc.Name}().");
                    return Console.Read();
                }
                else if (fc.Name == "getk")
                {
                    if (fc.Args.Count > 0) throw new Exception($"Invalid argument for {fc.Name}().");
                    return Console.ReadKey();
                }

                if (!_functions.TryGetValue(fc.Name, out var fdef))
                    throw new Exception($"Function not defined: {fc.Name}");
                var args = new List<object?>();
                foreach (var a in fc.Args) args.Add(Eval(a));
                return CallFunction(fdef, args);

            case BinOpNode b:
                var left = Eval(b.Left);

                if (b.Op == TokenType.And)
                {
                    bool lval = Convert.ToBoolean(left);
                    if (!lval) return false;
                    var r = Eval(b.Right);
                    return Convert.ToBoolean(r);
                }

                if (b.Op == TokenType.Or)
                {
                    bool lval = Convert.ToBoolean(left);
                    if (lval) return true;
                    var r = Eval(b.Right);
                    return Convert.ToBoolean(r);
                }

                var right = Eval(b.Right);

                if (b.Op == TokenType.Plus)
                {
                    if (left is string || right is string)
                        return left + right.ToString();
                    return Convert.ToDouble(left) + Convert.ToDouble(right);
                }

                dynamic? lft = left;
                dynamic? rght = right;

                return b.Op switch
                {
                    TokenType.ShiftLeft => (long)lft << (int)rght,
                    TokenType.ShiftRight => (long)lft >> (int)rght,
                    TokenType.Minus => lft - rght,
                    TokenType.Mul => lft * rght,
                    TokenType.Div => lft / rght,
                    TokenType.Pow => Math.Pow((double)lft, (double)rght),
                    TokenType.Mod => lft % rght,
                    TokenType.BitAnd => (long)lft & (long)rght,
                    TokenType.BitXor => (long)lft ^ (long)rght,
                    TokenType.BitOr => (long)lft | (long)rght,
                    TokenType.Equals => lft == rght,
                    TokenType.NotEquals => lft != rght,
                    TokenType.Less => lft < rght,
                    TokenType.LessEq => lft <= rght,
                    TokenType.Greater => lft > rght,
                    TokenType.GreaterEq => lft >= rght,
                    _ => throw new Exception($"Unknown binary operator at line {node.Line}, column {node.Column}.")
                };


            case UnaryOpNode u:
                dynamic? val = Eval(u.Node);
                return u.Op switch
                {
                    TokenType.Minus => -val,
                    _ => throw new Exception($"Unknown unary operator at line {node.Line}, column {node.Column}.")
                };

            default:
                throw new Exception($"Unknown node type {node.GetType().Name} at line {node.Line}, column {node.Column}");
        }
    }
}
