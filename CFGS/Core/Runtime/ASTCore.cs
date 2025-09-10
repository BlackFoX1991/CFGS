using CFGS.Core.Analytics;
using System.ComponentModel.DataAnnotations.Schema;

namespace CFGS.Core.Runtime;

#region AST_NODES

public abstract class Node
{
    public abstract int Column { get; set; }
    public abstract int Line { get; set; }
}

public class NumberNode(double val, int column, int line) : Node
{
    public readonly double Value = val;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class StringNode(string val, int column, int line) : Node
{
    public readonly string Value = val;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class CharNode(char val, int column, int line) : Node
{
    public readonly char Value = val;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class BoolNode(bool val, int column, int line) : Node
{
    public readonly bool Value = val;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class VarNode(string name, int column, int line) : Node
{
    public readonly string Name = name;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class BinOpNode(Node l, TokenType op, Node r, int column, int line) : Node
{
    public readonly Node Left = l;
    public readonly Node Right = r;
    public readonly TokenType Op = op;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class UnaryOpNode(TokenType op, Node node, int column, int line) : Node
{
    public readonly Node Node = node;
    public readonly TokenType Op = op;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class AssignNode(Node target, Node value, int column, int line) : Node
{
    public readonly Node Target = target; // kann VarNode oder ArrayAccessNode sein
    public readonly Node Value = value;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class ArrayNode(List<Node> elements, int column, int line) : Node
{
    public readonly List<Node> Elements = elements;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class ArrayAccessNode(Node array, Node index, int column, int line) : Node
{
    public readonly Node Array = array;
    public readonly Node Index = index;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class BlockNode(List<Node> stmts, int column, int line) : Node
{
    public readonly List<Node> Statements = stmts;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class IfNode(Node cond, Node thenBody, int column, int line, Node? elseBody = null) : Node
{
    public readonly Node Condition = cond;
    public readonly Node Body = thenBody;
    public readonly Node? ElseBody = elseBody; // neu, optional
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class WhileNode(Node cond, Node body, int column, int line) : Node
{
    public readonly Node Condition = cond;
    public readonly Node Body = body;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class BreakNode(int column, int line) : Node
{
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;

}

public class ContinueNode(int column, int line) : Node
{
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}



public class PrintNode(Node val, int column, int line) : Node
{
    public readonly Node Value = val;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class ImportNode(string file, int column, int line) : Node
{
    public readonly string FileName = file;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class FuncDefNode(string name, List<string> parameters, BlockNode body, int column, int line) : Node
{
    public readonly string Name = name;
    public readonly List<string> Params = parameters;
    public readonly BlockNode Body = body;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class FuncCallNode(string name, List<Node> args, int column, int line) : Node
{
    public readonly string Name = name;
    public readonly List<Node> Args = args;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class ReturnNode(Node value, int column, int line) : Node
{
    public readonly Node Value = value;
    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class StructDefNode(string name, List<string> fields, int column, int line) : Node
{
    public string Name { get; } = name;
    public List<string> Fields { get; } = fields;

    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class StructInstanceNode(string structName, List<Node> fieldValues, int column, int line) : Node
{
    public string StructName { get; } = structName;
    public List<Node> FieldValues { get; } = fieldValues;

    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

public class MemberAccessNode(Node objNode, string memberName, int column, int line) : Node
{
    public Node ObjectNode { get; } = objNode;
    public string MemberName { get; } = memberName;

    public override int Column { get; set; } = column;
    public override int Line { get; set; } = line;
}

#endregion