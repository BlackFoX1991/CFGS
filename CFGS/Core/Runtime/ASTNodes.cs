using CFGS.Core.Analytics;

namespace CFGS.Core.Runtime
{
    /// <summary>
    /// Defines the <see cref="EnumAccessNode" />
    /// </summary>
    public class EnumAccessNode : Node
    {
        /// <summary>
        /// Defines the EnumName
        /// </summary>
        public readonly string EnumName;

        /// <summary>
        /// Defines the MemberName
        /// </summary>
        public readonly string MemberName;

        /// <summary>
        /// Gets or sets the Column
        /// </summary>
        public override int Column { get; set; }

        /// <summary>
        /// Gets or sets the Line
        /// </summary>
        public override int Line { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumAccessNode"/> class.
        /// </summary>
        /// <param name="enumName">The enumName<see cref="string"/></param>
        /// <param name="memberName">The memberName<see cref="string"/></param>
        /// <param name="column">The column<see cref="int"/></param>
        /// <param name="line">The line<see cref="int"/></param>
        public EnumAccessNode(string enumName, string memberName, int column, int line)
        {
            EnumName = enumName;
            MemberName = memberName;
            Column = column;
            Line = line;
        }

        /// <summary>
        /// The ToString
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString() => $"{EnumName}.{MemberName}";
    }
}

/// <summary>
/// Defines the <see cref="AppendNode" />
/// </summary>
public class AppendNode : Node
{
    /// <summary>
    /// Defines the Array
    /// </summary>
    public readonly Node Array;

    /// <summary>
    /// Defines the Value
    /// </summary>
    public readonly Node Value;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppendNode"/> class.
    /// </summary>
    /// <param name="array">The array<see cref="Node"/></param>
    /// <param name="value">The value<see cref="Node"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public AppendNode(Node array, Node value, int column, int line)
    {
        Array = array;
        Value = value;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"{Array.ToString()}[] = {Value.ToString()}";
}

/// <summary>
/// Defines the <see cref="ArrayAccessNode" />
/// </summary>
public class ArrayAccessNode : Node
{
    /// <summary>
    /// Defines the Array
    /// </summary>
    public readonly Node Array;

    /// <summary>
    /// Defines the Index
    /// </summary>
    public readonly Node? Index;// ← nullable

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayAccessNode"/> class.
    /// </summary>
    /// <param name="array">The array<see cref="Node"/></param>
    /// <param name="index">The index<see cref="Node?"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public ArrayAccessNode(Node array, Node? index, int column, int line)
    {
        Array = array;
        Index = index;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => Index == null ? $"{Array}[]" : $"{Array}[{Index}]";
}

/// <summary>
/// Defines the <see cref="ArrayDeleteNode" />
/// </summary>
public class ArrayDeleteNode : Node
{
    /// <summary>
    /// Defines the Array
    /// </summary>
    public readonly Node Array;

    /// <summary>
    /// Defines the Index
    /// </summary>
    public readonly Node? Index;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayDeleteNode"/> class.
    /// </summary>
    /// <param name="array">The array<see cref="Node"/></param>
    /// <param name="index">The index<see cref="Node"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public ArrayDeleteNode(Node array, Node index, int column, int line)
    {
        Array = array;
        Index = index;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"delete {Array}[{Index}]";
}

/// <summary>
/// Defines the <see cref="ArrayNode" />
/// </summary>
public class ArrayNode : Node
{
    /// <summary>
    /// Defines the Elements
    /// </summary>
    public readonly List<Node> Elements;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayNode"/> class.
    /// </summary>
    /// <param name="elements">The elements<see cref="List{Node}"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public ArrayNode(List<Node> elements, int column, int line)
    {
        Elements = elements;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString()
    {
        return "[" + string.Join(", ", Elements.Select(e =>
        {
            if (e is StringNode s)
                return s.Value; // oder $"\"{s.Value}\"" falls mit Quotes
            return e.ToString();
        })) + "]";
    }
}

/// <summary>
/// Defines the <see cref="AssignNode" />
/// </summary>
public class AssignNode : Node
{
    /// <summary>
    /// Defines the Target
    /// </summary>
    public readonly Node Target;

    /// <summary>
    /// Defines the Value
    /// </summary>
    public readonly Node Value;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignNode"/> class.
    /// </summary>
    /// <param name="target">The target<see cref="Node"/></param>
    /// <param name="value">The value<see cref="Node"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public AssignNode(Node target, Node value, int column, int line)
    {
        Target = target;
        Value = value;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"({Target.ToString()} = {Value.ToString()})";
}

/// <summary>
/// Defines the <see cref="BinOpNode" />
/// </summary>
public class BinOpNode : Node
{
    /// <summary>
    /// Defines the Left
    /// </summary>
    public readonly Node Left;

    /// <summary>
    /// Defines the Right
    /// </summary>
    public readonly Node Right;

    /// <summary>
    /// Defines the Op
    /// </summary>
    public readonly TokenType Op;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinOpNode"/> class.
    /// </summary>
    /// <param name="l">The l<see cref="Node"/></param>
    /// <param name="op">The op<see cref="TokenType"/></param>
    /// <param name="r">The r<see cref="Node"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public BinOpNode(Node l, TokenType op, Node r, int column, int line)
    {
        Left = l;
        Op = op;
        Right = r;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"({Left.ToString()} {Op} {Right.ToString()})";
}

/// <summary>
/// Defines the <see cref="BlockNode" />
/// </summary>
public class BlockNode : Node
{
    /// <summary>
    /// Defines the Statements
    /// </summary>
    public readonly List<Node> Statements;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BlockNode"/> class.
    /// </summary>
    /// <param name="stmts">The stmts<see cref="List{Node}"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public BlockNode(List<Node> stmts, int column, int line)
    {
        Statements = stmts;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"{{{string.Join("; ", Statements.Select(s => s.ToString()))}}}";
}

/// <summary>
/// Defines the <see cref="BoolNode" />
/// </summary>
public class BoolNode : Node
{
    /// <summary>
    /// Defines the Value
    /// </summary>
    public readonly bool Value;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoolNode"/> class.
    /// </summary>
    /// <param name="val">The val<see cref="bool"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public BoolNode(bool val, int column, int line)
    {
        Value = val;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => Value.ToString();
}

/// <summary>
/// Defines the <see cref="BreakNode" />
/// </summary>
public class BreakNode : Node
{
    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BreakNode"/> class.
    /// </summary>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public BreakNode(int column, int line)
    {
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => "break";
}

/// <summary>
/// Defines the <see cref="CharNode" />
/// </summary>
public class CharNode : Node
{
    /// <summary>
    /// Defines the Value
    /// </summary>
    public readonly char Value;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CharNode"/> class.
    /// </summary>
    /// <param name="val">The val<see cref="char"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public CharNode(char val, int column, int line)
    {
        Value = val;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"'{Value}'";
}

/// <summary>
/// Defines the <see cref="ContinueNode" />
/// </summary>
public class ContinueNode : Node
{
    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContinueNode"/> class.
    /// </summary>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public ContinueNode(int column, int line)
    {
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => "continue";
}

/// <summary>
/// Defines the <see cref="EnumDefNode" />
/// </summary>
public class EnumDefNode : Node
{
    /// <summary>
    /// Defines the Name
    /// </summary>
    public readonly string Name;

    // int? für optionale Werte; null bedeutet, dass der Wert automatisch hochgezählt wird

    /// <summary>
    /// Defines the Members
    /// </summary>
    public readonly List<KeyValuePair<string, int?>> Members;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumDefNode"/> class.
    /// </summary>
    /// <param name="name">The name<see cref="string"/></param>
    /// <param name="members">The members<see cref="List{KeyValuePair{string, int?}}"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public EnumDefNode(string name, List<KeyValuePair<string, int?>> members, int column, int line)
    {
        Name = name;
        Members = members;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString()
    {
        var membersStr = string.Join(", ", Members.Select(m => m.Value.HasValue ? $"{m.Key}={m.Value}" : m.Key));
        return $"enum {Name} {{ {membersStr} }}";
    }
}

/// <summary>
/// Defines the <see cref="FuncCallNode" />
/// </summary>
public class FuncCallNode : Node
{
    /// <summary>
    /// Defines the Name
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Defines the Args
    /// </summary>
    public readonly List<Node> Args;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FuncCallNode"/> class.
    /// </summary>
    /// <param name="name">The name<see cref="string"/></param>
    /// <param name="args">The args<see cref="List{Node}"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public FuncCallNode(string name, List<Node> args, int column, int line)
    {
        Name = name;
        Args = args;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"{Name}({string.Join(", ", Args.Select(a => a.ToString()))})";
}

/// <summary>
/// Defines the <see cref="FuncDefNode" />
/// </summary>
public class FuncDefNode : Node
{
    /// <summary>
    /// Defines the Name
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Defines the Params
    /// </summary>
    public readonly List<string> Params;

    /// <summary>
    /// Defines the Body
    /// </summary>
    public readonly BlockNode Body;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FuncDefNode"/> class.
    /// </summary>
    /// <param name="name">The name<see cref="string"/></param>
    /// <param name="parameters">The parameters<see cref="List{string}"/></param>
    /// <param name="body">The body<see cref="BlockNode"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public FuncDefNode(string name, List<string> parameters, BlockNode body, int column, int line)
    {
        Name = name;
        Params = parameters;
        Body = body;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"func {Name}({string.Join(", ", Params)}) {Body.ToString()}";
}

/// <summary>
/// Defines the <see cref="IfNode" />
/// </summary>
public class IfNode : Node
{
    /// <summary>
    /// Defines the Condition
    /// </summary>
    public readonly Node Condition;

    /// <summary>
    /// Defines the Body
    /// </summary>
    public readonly Node Body;

    /// <summary>
    /// Defines the ElseBody
    /// </summary>
    public readonly Node? ElseBody;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IfNode"/> class.
    /// </summary>
    /// <param name="cond">The cond<see cref="Node"/></param>
    /// <param name="thenBody">The thenBody<see cref="Node"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    /// <param name="elseBody">The elseBody<see cref="Node?"/></param>
    public IfNode(Node cond, Node thenBody, int column, int line, Node? elseBody = null)
    {
        Condition = cond;
        Body = thenBody;
        ElseBody = elseBody;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() =>
        ElseBody == null
            ? $"if ({Condition.ToString()}) {Body.ToString()}"
            : $"if ({Condition.ToString()}) {Body.ToString()} else {ElseBody.ToString()}";
}

/// <summary>
/// Defines the <see cref="ImportNode" />
/// </summary>
public class ImportNode : Node
{
    /// <summary>
    /// Defines the FileName
    /// </summary>
    public readonly string FileName;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportNode"/> class.
    /// </summary>
    /// <param name="file">The file<see cref="string"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public ImportNode(string file, int column, int line)
    {
        FileName = file;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"import {FileName}";
}

/// <summary>
/// Defines the <see cref="MatchCaseNode" />
/// </summary>
public class MatchCaseNode : Node
{
    /// <summary>
    /// Defines the Values
    /// </summary>
    public readonly List<Node> Values;

    /// <summary>
    /// Defines the Body
    /// </summary>
    public readonly Node Body;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchCaseNode"/> class.
    /// </summary>
    /// <param name="values">The values<see cref="List{Node}"/></param>
    /// <param name="body">The body<see cref="Node"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public MatchCaseNode(List<Node> values, Node body, int column, int line)
    {
        Values = values;
        Body = body;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() =>
        $"case [{string.Join(", ", Values.Select(v => v.ToString()))}] => {Body.ToString()}";
}

/// <summary>
/// Defines the <see cref="MatchNode" />
/// </summary>
public class MatchNode : Node
{
    /// <summary>
    /// Defines the Value
    /// </summary>
    public readonly Node Value;

    /// <summary>
    /// Defines the Cases
    /// </summary>
    public readonly List<MatchCaseNode> Cases;

    /// <summary>
    /// Defines the DefaultCase
    /// </summary>
    public readonly Node? DefaultCase;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchNode"/> class.
    /// </summary>
    /// <param name="value">The value<see cref="Node"/></param>
    /// <param name="cases">The cases<see cref="List{MatchCaseNode}"/></param>
    /// <param name="defaultCase">The defaultCase<see cref="Node?"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public MatchNode(Node value, List<MatchCaseNode> cases, Node? defaultCase, int column, int line)
    {
        Value = value;
        Cases = cases;
        DefaultCase = defaultCase;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString()
    {
        var casesStr = string.Join("; ", Cases.Select(c => c.ToString()));
        var defaultStr = DefaultCase != null ? $" default => {DefaultCase.ToString()}" : "";
        return $"match ({Value.ToString()}) {{ {casesStr}{defaultStr} }}";
    }
}

/// <summary>
/// Defines the <see cref="MemberAccessNode" />
/// </summary>
public class MemberAccessNode : Node
{
    /// <summary>
    /// Defines the ObjectNode
    /// </summary>
    public readonly Node ObjectNode;

    /// <summary>
    /// Defines the MemberName
    /// </summary>
    public readonly string MemberName;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemberAccessNode"/> class.
    /// </summary>
    /// <param name="objNode">The objNode<see cref="Node"/></param>
    /// <param name="memberName">The memberName<see cref="string"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public MemberAccessNode(Node objNode, string memberName, int column, int line)
    {
        ObjectNode = objNode;
        MemberName = memberName;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"{ObjectNode.ToString()}.{MemberName}";
}

/// <summary>
/// Defines the <see cref="Node" />
/// </summary>
public abstract class Node
{
    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public abstract int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public abstract int Line { get; set; }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public abstract override string ToString();
}

/// <summary>
/// Defines the <see cref="NumberNode" />
/// </summary>
public class NumberNode : Node
{
    /// <summary>
    /// Defines the Value
    /// </summary>
    public readonly double Value;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NumberNode"/> class.
    /// </summary>
    /// <param name="val">The val<see cref="double"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public NumberNode(double val, int column, int line)
    {
        Value = val;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => Value.ToString();
}

/// <summary>
/// Defines the <see cref="PrintCharNode" />
/// </summary>
public class PrintCharNode : Node
{
    /// <summary>
    /// Defines the Value
    /// </summary>
    public readonly Node Value;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintCharNode"/> class.
    /// </summary>
    /// <param name="val">The val<see cref="Node"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public PrintCharNode(Node val, int column, int line)
    {
        Value = val;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"printc({Value.ToString()})";
}

/// <summary>
/// Defines the <see cref="PrintNode" />
/// </summary>
public class PrintNode : Node
{
    /// <summary>
    /// Defines the Value
    /// </summary>
    public readonly Node Value;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintNode"/> class.
    /// </summary>
    /// <param name="val">The val<see cref="Node"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public PrintNode(Node val, int column, int line)
    {
        Value = val;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"print({Value.ToString()})";
}

/// <summary>
/// Defines the <see cref="ReturnNode" />
/// </summary>
public class ReturnNode : Node
{
    /// <summary>
    /// Defines the Value
    /// </summary>
    public readonly Node Value;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReturnNode"/> class.
    /// </summary>
    /// <param name="value">The value<see cref="Node"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public ReturnNode(Node value, int column, int line)
    {
        Value = value;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"return {Value.ToString()}";
}

/// <summary>
/// Defines the <see cref="SliceNode" />
/// </summary>
public class SliceNode : Node
{
    /// <summary>
    /// Defines the Target
    /// </summary>
    public readonly Node Target;

    /// <summary>
    /// Defines the Start
    /// </summary>
    public readonly Node? Start;

    /// <summary>
    /// Defines the End
    /// </summary>
    public readonly Node? End;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SliceNode"/> class.
    /// </summary>
    /// <param name="target">The target<see cref="Node"/></param>
    /// <param name="start">The start<see cref="Node"/></param>
    /// <param name="end">The end<see cref="Node"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public SliceNode(Node target, Node start, Node end, int column, int line)
    {
        Target = target;
        Start = start;
        End = end;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"{Target.ToString()}[{Start?.ToString()}:{End?.ToString()}]";
}

/// <summary>
/// Defines the <see cref="StringNode" />
/// </summary>
public class StringNode : Node
{
    /// <summary>
    /// Defines the Value
    /// </summary>
    public readonly string Value;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringNode"/> class.
    /// </summary>
    /// <param name="val">The val<see cref="string"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public StringNode(string val, int column, int line)
    {
        Value = val;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"\"{Value}\"";
}

/// <summary>
/// Defines the <see cref="StructDefNode" />
/// </summary>
public class StructDefNode : Node
{
    /// <summary>
    /// Defines the Name
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Defines the Fields
    /// </summary>
    public readonly List<string> Fields;

    /// <summary>
    /// Defines the Methods
    /// </summary>
    public readonly List<Node>? Methods;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StructDefNode"/> class.
    /// </summary>
    /// <param name="name">The name<see cref="string"/></param>
    /// <param name="fields">The fields<see cref="List{string}"/></param>
    /// <param name="methods">The methods<see cref="List{Node}?"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public StructDefNode(string name, List<string> fields, List<Node>? methods, int column, int line)
    {
        Name = name;
        Fields = fields;
        Methods = methods;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"struct {Name} {{ {string.Join(", ", Fields)} }}";
}

/// <summary>
/// Defines the <see cref="StructInstanceNode" />
/// </summary>
public class StructInstanceNode : Node
{
    /// <summary>
    /// Defines the StructName
    /// </summary>
    public readonly string StructName;

    /// <summary>
    /// Defines the FieldValues
    /// </summary>
    public readonly List<Node> FieldValues;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StructInstanceNode"/> class.
    /// </summary>
    /// <param name="structName">The structName<see cref="string"/></param>
    /// <param name="fieldValues">The fieldValues<see cref="List{Node}"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public StructInstanceNode(string structName, List<Node> fieldValues, int column, int line)
    {
        StructName = structName;
        FieldValues = fieldValues;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() =>
        $"{StructName} {{ {string.Join(", ", FieldValues.Select(f => f.ToString()))} }}";
}

/// <summary>
/// Defines the <see cref="ThrowNode" />
/// </summary>
public class ThrowNode : Node
{
    /// <summary>
    /// Defines the Value
    /// </summary>
    public readonly Node Value;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThrowNode"/> class.
    /// </summary>
    /// <param name="value">The value<see cref="Node"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public ThrowNode(Node value, int column, int line)
    {
        Value = value;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"throw {Value.ToString()}";
}

/// <summary>
/// Defines the <see cref="TryNode" />
/// </summary>
public class TryNode : Node
{
    /// <summary>
    /// Defines the TryBlock
    /// </summary>
    public readonly Node TryBlock;

    /// <summary>
    /// Defines the CatchVar
    /// </summary>
    public readonly Node? CatchVar;

    /// <summary>
    /// Defines the CatchBlock
    /// </summary>
    public readonly Node? CatchBlock;

    /// <summary>
    /// Defines the FinallyBlock
    /// </summary>
    public readonly Node? FinallyBlock;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TryNode"/> class.
    /// </summary>
    /// <param name="tryBlock">The tryBlock<see cref="Node"/></param>
    /// <param name="catchVar">The catchVar<see cref="Node?"/></param>
    /// <param name="catchBlock">The catchBlock<see cref="Node?"/></param>
    /// <param name="finallyBlock">The finallyBlock<see cref="Node?"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public TryNode(Node tryBlock, Node? catchVar, Node? catchBlock, Node? finallyBlock, int column, int line)
    {
        TryBlock = tryBlock;
        CatchVar = catchVar;
        CatchBlock = catchBlock;
        FinallyBlock = finallyBlock;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
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

/// <summary>
/// Defines the <see cref="UnaryOpNode" />
/// </summary>
public class UnaryOpNode : Node
{
    /// <summary>
    /// Gets the Op
    /// </summary>
    public TokenType Op { get; }

    /// <summary>
    /// Gets the Node
    /// </summary>
    public Node Node { get; }

    /// <summary>
    /// Gets a value indicating whether IsPrefix
    /// </summary>
    public bool IsPrefix { get; }

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnaryOpNode"/> class.
    /// </summary>
    /// <param name="op">The op<see cref="TokenType"/></param>
    /// <param name="node">The node<see cref="Node"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    /// <param name="isPrefix">The isPrefix<see cref="bool"/></param>
    public UnaryOpNode(TokenType op, Node node, int column, int line, bool isPrefix = true)
    {
        Op = op;
        Node = node;
        IsPrefix = isPrefix;

        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"({Op}{Node.ToString()})";
}

/// <summary>
/// Defines the <see cref="VarNode" />
/// </summary>
public class VarNode : Node
{
    /// <summary>
    /// Defines the Name
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VarNode"/> class.
    /// </summary>
    /// <param name="name">The name<see cref="string"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public VarNode(string name, int column, int line)
    {
        Name = name;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => Name;
}

/// <summary>
/// Defines the <see cref="WhileNode" />
/// </summary>
public class WhileNode : Node
{
    /// <summary>
    /// Defines the Condition
    /// </summary>
    public readonly Node Condition;

    /// <summary>
    /// Defines the Body
    /// </summary>
    public readonly Node Body;

    /// <summary>
    /// Gets or sets the Column
    /// </summary>
    public override int Column { get; set; }

    /// <summary>
    /// Gets or sets the Line
    /// </summary>
    public override int Line { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WhileNode"/> class.
    /// </summary>
    /// <param name="cond">The cond<see cref="Node"/></param>
    /// <param name="body">The body<see cref="Node"/></param>
    /// <param name="column">The column<see cref="int"/></param>
    /// <param name="line">The line<see cref="int"/></param>
    public WhileNode(Node cond, Node body, int column, int line)
    {
        Condition = cond;
        Body = body;
        Column = column;
        Line = line;
    }

    /// <summary>
    /// The ToString
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString() => $"while ({Condition.ToString()}) {Body.ToString()}";
}
