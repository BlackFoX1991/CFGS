using CFGS.Core.Analytics;
using CFGS.Core.Runtime.AST;
using CFGS.Core.Runtime.Instances;

namespace CFGS.Core.Runtime;

#pragma warning disable CS8602
#pragma warning disable CS8604
public class Interpreter
{
    private readonly List<Dictionary<string, object?>> _scopes = new();
    private readonly Dictionary<string, FuncDefNode> _functions = new();
    private readonly Dictionary<string, StructDefNode> _structs = new();
    private readonly HashSet<string> _importedFiles = new();
    private readonly Dictionary<string, EnumDef> enums = new();

    public Interpreter()
    {
        _scopes.Add(new Dictionary<string, object?>()); // global scope
    }

    private void PushScope() => _scopes.Add(new Dictionary<string, object?>());
    private void PopScope() => _scopes.RemoveAt(_scopes.Count - 1);

    private object? GetVariable(string name, int ln, int col)
    {
        for (int i = _scopes.Count - 1; i >= 0; i--)
            if (_scopes[i].ContainsKey(name))
                return _scopes[i][name];
        throw new Exception($"Variable {name} not defined, line {ln}, column {col}.");
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
                {
                    var val = Eval(p.Value);
                    Console.WriteLine(ValueFormat.FormatValue(val));

                    break;
                }
            case PrintCharNode p:
                {
                    var val = Eval(p.Value);
                    Console.Write(ValueFormat.FormatValue(val));

                    break;
                }

            case AppendNode append:
                {
                    var arrObj = Eval(append.Array);

                    if (arrObj is not List<object?> list)
                        throw new Exception($"Trying to append to a non-array, line {append.Line}, column {append.Column}.");

                    var val = Eval(append.Value);
                    list.Add(val);
                    break;
                }

            case ArrayDeleteNode del:
                {
                    var arrObj = Eval(del.Array);
                    if (arrObj is not List<object?> list)
                        throw new Exception($"Trying to delete from a non-array, line {del.Line}, column {del.Column}.");

                    if (del.Index == null)
                    {
                        // Index fehlt → alle Elemente löschen
                        list.Clear();
                    }
                    else
                    {
                        var idx = Convert.ToInt32(Eval(del.Index));
                        if (idx < 0 || idx >= list.Count)
                            throw new Exception($"Array index out of bounds, line {del.Line}, column {del.Column}.");

                        list.RemoveAt(idx);
                    }
                    break;
                }

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
                            throw new Exception($"Trying to index a non-array, line {a.Line}, column {a.Column}.");

                        var idxObj = Eval(aa.Index);
                        int idx = Convert.ToInt32(idxObj);

                        if (idx < 0 || idx >= list.Count)
                            throw new Exception($"Array index out of bounds, line {a.Line}, column {a.Column}.");

                        list[idx] = val;
                    }
                    else if (a.Target is MemberAccessNode ma)
                    {
                        var obj = Eval(ma.ObjectNode);
                        if (obj is StructInstance si)
                        {
                            if (!si.Fields.ContainsKey(ma.MemberName))
                                throw new Exception($"Struct {si.Name} has no field {ma.MemberName}, line {ma.Line}, column {ma.Column}.");
                            si.Fields[ma.MemberName] = val;
                        }
                        else
                        {
                            throw new Exception($"Invalid assignment target (not a struct instance), line {ma.Line}, column {ma.Column}.");
                        }
                    }
                    else if (a.Target is SliceNode s)
                    {
                        var targetObj = Eval(s.Target);
                        int start = Convert.ToInt32(Eval(s.Start));
                        int end = Convert.ToInt32(Eval(s.End));
                        var valueObj = Eval(a.Value);

                        if (targetObj is List<object?> list)
                        {
                            if (valueObj is not List<object?> newList)
                                throw new Exception($"Slice assignment requires an array/list, line {a.Line}, column {a.Column}.");

                            if (start < 0) start = 0;
                            if (end > list.Count) end = list.Count;

                            int count = end - start;
                            if (newList.Count != count)
                                throw new Exception($"Slice assignment length mismatch, line {a.Line}, column {a.Column}.");

                            for (int i = 0; i < count; i++)
                                list[start + i] = newList[i];
                        }
                        else if (targetObj is string str)
                        {
                            if (valueObj is not string newStr)
                                throw new Exception($"Slice assignment requires a string, line {a.Line} , column  {a.Column}.");

                            if (start < 0) start = 0;
                            if (end > str.Length) end = str.Length;

                            if ((end - start) != newStr.Length)
                                throw new Exception($"Slice assignment length mismatch, line {a.Line}  , column   {a.Column}.");

                            var chars = str.ToCharArray();
                            for (int i = 0; i < newStr.Length; i++)
                                chars[start + i] = newStr[i];

                            // Falls Variable direkt ein VarNode ist:
                            if (s.Target is VarNode vn)
                                SetVariable(vn.Name, new string(chars));
                            else
                                throw new Exception($"Unsupported slice target for string assignment, line {a.Line}  , column   {a.Column}.");
                        }
                        else
                        {
                            throw new Exception($"Slice assignment can only be applied to arrays or strings, line {a.Line}  , column   {a.Column}.");
                        }
                    }

                    else
                    {
                        throw new Exception($"Invalid assignment target, line {a.Line}   , column    {a.Column}.");
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
                VisitGlobals(tree);
                break;

            case FuncDefNode f:
                if (_functions.ContainsKey(f.Name))
                    throw new Exception($"Function '{f.Name}' declared more than once. Line {f.Line}, column {f.Column}.");
                _functions[f.Name] = f;
                break;

            case StructDefNode s:
                if (_structs.ContainsKey(s.Name))
                    throw new Exception($"Struct '{s.Name}' declared more than once. Line {s.Line}, column {s.Column}.");
                _structs[s.Name] = s;
                break;

            case FuncCallNode call:
                Eval(call); // function call as statement
                break;

            case ReturnNode r:
                throw new cException.ReturnException(Eval(r.Value));

            case EnumDefNode e:
                {
                    if (enums.ContainsKey(e.Name))
                        throw new Exception($"Enum '{e.Name}' declared more than once. Line {e.Line}, column {e.Column}.");

                    // EnumDef aus EnumDefNode erstellen, automatische Werte werden zugewiesen
                    var enumDef = new EnumDef(e.Name, e.Members);

                    // Enum in Dictionary speichern
                    enums[e.Name] = enumDef;

                    // Enum als Variable verfügbar machen
                    SetVariable(e.Name, enumDef);
                    break;
                }

            default:
                Eval(node);
                break;
        }
    }

    public void VisitGlobals(Node node)
    {
        switch (node)
        {
            case EnumDefNode e:
                {
                    if (enums.ContainsKey(e.Name))
                        throw new Exception($"Enum '{e.Name}' declared more than once. Line {e.Line}, column {e.Column}.");
                    // EnumDef aus EnumDefNode erstellen, automatische Werte werden zugewiesen
                    var enumDef = new EnumDef(e.Name, e.Members);

                    // Enum in Dictionary speichern
                    enums[e.Name] = enumDef;

                    // Enum als Variable verfügbar machen
                    SetVariable(e.Name, enumDef);
                    break;
                }

            case FuncDefNode f:
                if (_functions.ContainsKey(f.Name))
                    throw new Exception($"Function '{f.Name}' declared more than once. Line {f.Line}, column {f.Column}.");
                _functions[f.Name] = f;
                break;
            case StructDefNode s:
                if (_structs.ContainsKey(s.Name))
                    throw new Exception($"Struct '{s.Name}' declared more than once. Line {s.Line}, column {s.Column}.");
                _structs[s.Name] = s;
                break;
            case AssignNode a:
                Visit(a); // globale Variablen initialisieren
                break;
            case ImportNode imp:
                VisitGlobals(imp); // Imports ausführen
                break;
            case BlockNode b:
                foreach (var stmt in b.Statements)
                    VisitGlobals(stmt);
                break;
                // alles andere ignorieren
        }
    }

    public object? CallFunctionByName(string name, List<object?> args)
    {
        if (!_functions.TryGetValue(name, out var fdef))
            throw new Exception($"Function not defined: {name}");

        return CallFunction(fdef, args);
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
            case VarNode v: return GetVariable(v.Name, v.Line, v.Column);

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
                                    throw new Exception($"Array index out of bounds, line {ac.Line}, column {ac.Column}.");

                                return list[idx];
                            }
                        case string stx:
                            {
                                var idxStr = Eval(ac.Index);
                                int idx = Convert.ToInt32(idxStr);

                                if (idx < 0 || idx >= stx.Length)
                                    throw new Exception($"String index out of bounds, line {ac.Line}, column {ac.Column}.");

                                return Convert.ToChar(stx[idx]);
                            }
                        default:
                            throw new Exception($"Trying to index a non-array, line {ac.Line}, column {ac.Column}.");
                    }
                }
            case EnumAccessNode ea:
                if (!enums.TryGetValue(ea.EnumName, out var enumDef))
                    throw new Exception($"Enum {ea.EnumName} not defined, line {ea.Line}, column {ea.Column}.");

                if (!enumDef.Members.ContainsKey(ea.MemberName))
                    throw new Exception($"Enum {ea.EnumName} has no member {ea.MemberName}, line {ea.Line}, column {ea.Column}.");

                // Enum-Wert → z. B. Index zurückgeben
                return enumDef.Members[ea.MemberName];

            case StructInstanceNode si:
                if (!_structs.TryGetValue(si.StructName, out var sdef))
                    throw new Exception($"Struct not defined '{si.StructName}', line {si.Line}, column {si.Column}.");

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
                            throw new Exception($"Struct {sInst.Name} has no field '{ma.MemberName}', line {ma.Line}, column {ma.Column}.");
                        return eval;
                    }

                    if (obj is EnumDef eDef)
                    {
                        if (!eDef.Members.ContainsKey(ma.MemberName))
                            throw new Exception($"Enum {eDef.Name} has no member '{ma.MemberName}', line {ma.Line}, column {ma.Column}.");
                        return eDef.Members[ma.MemberName];
                    }

                    throw new Exception($"Member access on non-struct instance at line {ma.Line}, column {ma.Column}.");
                }

            case FuncCallNode fc:

                if (BuiltInFunctions.builtinfuncs.TryGetValue(fc.Name, out var bltin))
                    return bltin(fc.Args.Select(a => Eval(a)).ToList());

                if (!_functions.TryGetValue(fc.Name, out var fdef))
                    throw new Exception($"Function not defined: {fc.Name}, line {fc.Line}, column {fc.Column}.");
                var args = new List<object?>();
                foreach (var a in fc.Args) args.Add(Eval(a));
                return CallFunction(fdef, args);

            case BinOpNode b:
                var left = Eval(b.Left) ?? 0;

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

                var right = Eval(b.Right) ?? 0;

                if (b.Op == TokenType.Plus)
                {
                    // Wenn eines der beiden ein string ist, alles in string umwandeln
                    if (left is string || right is string)
                    {
                        string lstr = left is List<object> llist ? ValueFormat.FormatValue(llist) : left?.ToString() ?? "";
                        string rstr = right is List<object> rlist ? ValueFormat.FormatValue(rlist) : right?.ToString() ?? "";
                        return lstr + rstr;
                    }

                    // Wenn eines ein Array/Struct ist → String
                    if (left is List<object> llist2) return ValueFormat.FormatValue(llist2) + right?.ToString();
                    if (right is List<object> rlist2) return left?.ToString() + ValueFormat.FormatValue(rlist2);

                    // Standard: Zahl
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
                    TokenType.Equals => AreEqual(lft, rght),
                    TokenType.NotEquals => !AreEqual(lft, rght),
                    TokenType.Less => lft < rght,
                    TokenType.LessEq => lft <= rght,
                    TokenType.Greater => lft > rght,
                    TokenType.GreaterEq => lft >= rght,
                    _ => throw new Exception($"Unknown binary operator at line {b.Line}, column {b.Column}.")
                };
            case SliceNode s:
                {
                    var target = Eval(s.Target);

                    int start = s.Start != null ? Convert.ToInt32(Eval(s.Start)) : 0;
                    int end = s.End != null ? Convert.ToInt32(Eval(s.End)) : (target is List<object?> list ? list.Count : target is string str ? str.Length : 0);

                    if (target is List<object?> slist)
                    {
                        if (start < 0) start = 0;
                        if (end > slist.Count) end = slist.Count;
                        return slist.GetRange(start, end - start);
                    }
                    else if (target is string sstr)
                    {
                        if (start < 0) start = 0;
                        if (end > sstr.Length) end = sstr.Length;
                        return sstr.Substring(start, end - start);
                    }
                    else
                    {
                        throw new Exception($"Slice can only be applied to arrays or strings, line {s.Line}, column {s.Column}.");
                    }
                }

            case UnaryOpNode u:
                if (u.Op == TokenType.Minus)
                    return -Convert.ToDouble(Eval(u.Node));

                if (u.Op == TokenType.Not)
                    return !Convert.ToBoolean(Eval(u.Node));

                if (u.Op == TokenType.PlusPlus || u.Op == TokenType.MinusMinus)
                {
                    double oldVal;
                    double newVal;

                    switch (u.Node)
                    {
                        case VarNode v:
                            oldVal = Convert.ToDouble(GetVariable(v.Name, v.Line, v.Column));
                            newVal = u.Op == TokenType.PlusPlus ? oldVal + 1 : oldVal - 1;
                            SetVariable(v.Name, newVal);
                            return u.IsPrefix ? newVal : oldVal;

                        case ArrayAccessNode aa:
                            {
                                var arrObj = Eval(aa.Array);
                                if (arrObj is not List<object?> list)
                                    throw new Exception($"Trying to increment/decrement a non-array element, line {aa.Line}, column {aa.Column}.");

                                int idx = Convert.ToInt32(Eval(aa.Index));
                                if (idx < 0 || idx >= list.Count)
                                    throw new Exception($"Array index out of bounds, line {aa.Line}, column {aa.Column}.");

                                oldVal = Convert.ToDouble(list[idx]);
                                newVal = u.Op == TokenType.PlusPlus ? oldVal + 1 : oldVal - 1;
                                list[idx] = newVal;

                                return u.IsPrefix ? newVal : oldVal;
                            }

                        case MemberAccessNode ma:
                            {
                                var obj = Eval(ma.ObjectNode);
                                if (obj is not StructInstance si)
                                    throw new Exception($"Trying to increment/decrement a non-struct field, line {ma.Line}, column {ma.Column}.");

                                if (!si.Fields.ContainsKey(ma.MemberName))
                                    throw new Exception($"Struct {si.Name} has no field {ma.MemberName}");

                                oldVal = Convert.ToDouble(si.Fields[ma.MemberName]);
                                newVal = u.Op == TokenType.PlusPlus ? oldVal + 1 : oldVal - 1;
                                si.Fields[ma.MemberName] = newVal;

                                return u.IsPrefix ? newVal : oldVal;
                            }

                        default:
                            throw new Exception($"Increment/decrement can only be applied to variables, array elements, or struct fields, line {u.Line}, column {u.Column}.");
                    }
                }

                throw new Exception($"Unknown unary operator {u.Op} at line {u.Line}, column {u.Column}");

            default:
                throw new Exception($"Unknown node type {node.GetType().Name} at line {node.Line}, column {node.Column}");
        }
    }

    private bool AreEqual(object? a, object? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;

        if (a is StructInstance sa && b is StructInstance sb)
            return ReferenceEquals(sa, sb);

        if (a is List<object?> la && b is List<object?> lb)
            return ReferenceEquals(la, lb);

        if (a is char ca && b is char cb)
            return ca == cb;

        return a.Equals(b);
    }

}
