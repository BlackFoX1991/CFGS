using CFGS.Core.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CFGS.Core.Runtime
{
    #region AST_NODES

    public abstract class Node
    {
        public abstract int Column { get; set; }
        public abstract int Line { get; set; }
        public abstract override string ToString();
    }

    public class NumberNode : Node
    {
        public readonly double Value;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public NumberNode(double val, int column, int line)
        {
            Value = val;
            Column = column;
            Line = line;
        }

        public override string ToString() => Value.ToString();
    }

    public class StringNode : Node
    {
        public readonly string Value;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public StringNode(string val, int column, int line)
        {
            Value = val;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"\"{Value}\"";
    }

    public class CharNode : Node
    {
        public readonly char Value;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public CharNode(char val, int column, int line)
        {
            Value = val;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"'{Value}'";
    }

    public class BoolNode : Node
    {
        public readonly bool Value;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public BoolNode(bool val, int column, int line)
        {
            Value = val;
            Column = column;
            Line = line;
        }

        public override string ToString() => Value.ToString();
    }

    public class VarNode : Node
    {
        public readonly string Name;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public VarNode(string name, int column, int line)
        {
            Name = name;
            Column = column;
            Line = line;
        }

        public override string ToString() => Name;
    }

    public class BinOpNode : Node
    {
        public readonly Node Left;
        public readonly Node Right;
        public readonly TokenType Op;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public BinOpNode(Node l, TokenType op, Node r, int column, int line)
        {
            Left = l;
            Op = op;
            Right = r;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"({Left.ToString()} {Op} {Right.ToString()})";
    }

    public class UnaryOpNode : Node
    {
        public TokenType Op { get; }
        public Node Node { get; }
        public bool IsPrefix { get; }

        public override int Column { get; set; }
        public override int Line { get; set; }

        public UnaryOpNode(TokenType op, Node node, int column, int line, bool isPrefix = true) 
        {
            Op = op;
            Node = node;
            IsPrefix = isPrefix;

            Column = column;
            Line = line;    
        }
        public override string ToString() => $"({Op}{Node.ToString()})";
    }

    public class AssignNode : Node
    {
        public readonly Node Target;
        public readonly Node Value;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public AssignNode(Node target, Node value, int column, int line)
        {
            Target = target;
            Value = value;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"({Target.ToString()} = {Value.ToString()})";
    }

    public class ArrayNode : Node
    {
        public readonly List<Node> Elements;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public ArrayNode(List<Node> elements, int column, int line)
        {
            Elements = elements;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"[{string.Join(", ", Elements.Select(e => e.ToString()))}]";
    }

    public class ArrayAccessNode : Node
    {
        public readonly Node Array;
        public readonly Node? Index; // ← nullable
        public override int Column { get; set; }
        public override int Line { get; set; }

        public ArrayAccessNode(Node array, Node? index, int column, int line)
        {
            Array = array;
            Index = index;
            Column = column;
            Line = line;
        }

        public override string ToString() => Index == null ? $"{Array}[]" : $"{Array}[{Index}]";
    }

    public class ArrayDeleteNode : Node
    {
        public readonly Node Array;
        public readonly Node Index;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public ArrayDeleteNode(Node array, Node index, int column, int line)
        {
            Array = array;
            Index = index;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"delete {Array}[{Index}]";
    }



    public class SliceNode : Node
    {
        public readonly Node Target;
        public readonly Node Start;
        public readonly Node End;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public SliceNode(Node target, Node start, Node end, int column, int line)
        {
            Target = target;
            Start = start;
            End = end;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"{Target.ToString()}[{Start.ToString()}:{End.ToString()}]";
    }

    public class AppendNode : Node
    {
        public readonly Node Array;
        public readonly Node Value;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public AppendNode(Node array, Node value, int column, int line)
        {
            Array = array;
            Value = value;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"{Array.ToString()}[] = {Value.ToString()}";
    }


    public class BlockNode : Node
    {
        public readonly List<Node> Statements;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public BlockNode(List<Node> stmts, int column, int line)
        {
            Statements = stmts;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"{{{string.Join("; ", Statements.Select(s => s.ToString()))}}}";
    }

    public class IfNode : Node
    {
        public readonly Node Condition;
        public readonly Node Body;
        public readonly Node? ElseBody;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public IfNode(Node cond, Node thenBody, int column, int line, Node? elseBody = null)
        {
            Condition = cond;
            Body = thenBody;
            ElseBody = elseBody;
            Column = column;
            Line = line;
        }

        public override string ToString() =>
            ElseBody == null
                ? $"if ({Condition.ToString()}) {Body.ToString()}"
                : $"if ({Condition.ToString()}) {Body.ToString()} else {ElseBody.ToString()}";
    }

    public class WhileNode : Node
    {
        public readonly Node Condition;
        public readonly Node Body;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public WhileNode(Node cond, Node body, int column, int line)
        {
            Condition = cond;
            Body = body;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"while ({Condition.ToString()}) {Body.ToString()}";
    }

    public class BreakNode : Node
    {
        public override int Column { get; set; }
        public override int Line { get; set; }

        public BreakNode(int column, int line)
        {
            Column = column;
            Line = line;
        }

        public override string ToString() => "break";
    }

    public class ContinueNode : Node
    {
        public override int Column { get; set; }
        public override int Line { get; set; }

        public ContinueNode(int column, int line)
        {
            Column = column;
            Line = line;
        }

        public override string ToString() => "continue";
    }

    public class PrintNode : Node
    {
        public readonly Node Value;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public PrintNode(Node val, int column, int line)
        {
            Value = val;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"print({Value.ToString()})";
    }

    public class ImportNode : Node
    {
        public readonly string FileName;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public ImportNode(string file, int column, int line)
        {
            FileName = file;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"import {FileName}";
    }

    public class FuncDefNode : Node
    {
        public readonly string Name;
        public readonly List<string> Params;
        public readonly BlockNode Body;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public FuncDefNode(string name, List<string> parameters, BlockNode body, int column, int line)
        {
            Name = name;
            Params = parameters;
            Body = body;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"func {Name}({string.Join(", ", Params)}) {Body.ToString()}";
    }

    public class FuncCallNode : Node
    {
        public readonly string Name;
        public readonly List<Node> Args;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public FuncCallNode(string name, List<Node> args, int column, int line)
        {
            Name = name;
            Args = args;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"{Name}({string.Join(", ", Args.Select(a => a.ToString()))})";
    }

    public class ReturnNode : Node
    {
        public readonly Node Value;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public ReturnNode(Node value, int column, int line)
        {
            Value = value;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"return {Value.ToString()}";
    }

    public class StructDefNode : Node
    {
        public readonly string Name;
        public readonly List<string> Fields;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public StructDefNode(string name, List<string> fields, int column, int line)
        {
            Name = name;
            Fields = fields;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"struct {Name} {{ {string.Join(", ", Fields)} }}";
    }

    public class StructInstanceNode : Node
    {
        public readonly string StructName;
        public readonly List<Node> FieldValues;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public StructInstanceNode(string structName, List<Node> fieldValues, int column, int line)
        {
            StructName = structName;
            FieldValues = fieldValues;
            Column = column;
            Line = line;
        }

        public override string ToString() =>
            $"{StructName} {{ {string.Join(", ", FieldValues.Select(f => f.ToString()))} }}";
    }

    public class MemberAccessNode : Node
    {
        public readonly Node ObjectNode;
        public readonly string MemberName;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public MemberAccessNode(Node objNode, string memberName, int column, int line)
        {
            ObjectNode = objNode;
            MemberName = memberName;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"{ObjectNode.ToString()}.{MemberName}";
    }

    public class TryNode : Node
    {
        public readonly Node TryBlock;
        public readonly Node? CatchVar;
        public readonly Node? CatchBlock;
        public readonly Node? FinallyBlock;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public TryNode(Node tryBlock, Node? catchVar, Node? catchBlock, Node? finallyBlock, int column, int line)
        {
            TryBlock = tryBlock;
            CatchVar = catchVar;
            CatchBlock = catchBlock;
            FinallyBlock = finallyBlock;
            Column = column;
            Line = line;
        }

        public override string ToString()
        {
            var s = $"try {TryBlock.ToString()}";
            if (CatchVar != null || CatchBlock != null)
                s += $" catch ({CatchVar?.ToString() ?? ""}) {CatchBlock?.ToString() ?? ""}";
            if (FinallyBlock != null)
                s += $" finally {FinallyBlock.ToString()}";
            return s;
        }
    }

    public class ThrowNode : Node
    {
        public readonly Node Value;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public ThrowNode(Node value, int column, int line)
        {
            Value = value;
            Column = column;
            Line = line;
        }

        public override string ToString() => $"throw {Value.ToString()}";
    }

    public class MatchCaseNode : Node
    {
        public readonly List<Node> Values;
        public readonly Node Body;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public MatchCaseNode(List<Node> values, Node body, int column, int line)
        {
            Values = values;
            Body = body;
            Column = column;
            Line = line;
        }

        public override string ToString() =>
            $"case [{string.Join(", ", Values.Select(v => v.ToString()))}] => {Body.ToString()}";
    }

    public class MatchNode : Node
    {
        public readonly Node Value;
        public readonly List<MatchCaseNode> Cases;
        public readonly Node? DefaultCase;
        public override int Column { get; set; }
        public override int Line { get; set; }

        public MatchNode(Node value, List<MatchCaseNode> cases, Node? defaultCase, int column, int line)
        {
            Value = value;
            Cases = cases;
            DefaultCase = defaultCase;
            Column = column;
            Line = line;
        }

        public override string ToString()
        {
            var casesStr = string.Join("; ", Cases.Select(c => c.ToString()));
            var defaultStr = DefaultCase != null ? $" default => {DefaultCase.ToString()}" : "";
            return $"match ({Value.ToString()}) {{ {casesStr}{defaultStr} }}";
        }
    }

    #endregion
}
