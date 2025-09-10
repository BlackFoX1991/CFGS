using System.Globalization;
using CFGS.Core.Runtime;

namespace CFGS.Core.Analytics;

#pragma warning disable CS8602
public class Parser(List<Token> tokens)
{
    private int _pos;
    private Token? Current => _pos < tokens.Count ? tokens[_pos] : null;

    private void Eat(TokenType type)
    {
        if (Current.Type == type) _pos++;
        else throw new Exception($"Expected {type}, got {Current.Type} at line {Current.Line}, column {Current.Column}.");
    }

    public Node Parse() => Block();

    private Node Block()
    {
        var statements = new List<Node>();
        while (Current.Type != TokenType.Eof && Current.Type != TokenType.RBrace) statements.Add(Statement());
        return new BlockNode(statements, Current.Column, Current.Line);
    }

    private Node Statement()
    {
        // Kontrollstrukturen / Keywords zuerst
        if (Current.Type == TokenType.Print)
        {
            Eat(TokenType.Print);
            Eat(TokenType.LParen);
            var val = Expr();
            Eat(TokenType.RParen);
            Eat(TokenType.Semicolon);
            return new PrintNode(val, Current.Column, Current.Line);
        }
        else if (Current.Type == TokenType.Import)
        {
            Eat(TokenType.Import);
            if (Current.Type != TokenType.String) throw new Exception("Expected string filename after import");
            string filename = Current.Value;
            Eat(TokenType.String);
            Eat(TokenType.Semicolon);
            return new ImportNode(filename, Current.Column, Current.Line);
        }
        else if (Current.Type == TokenType.Func)
        {
            Eat(TokenType.Func);
            string name = Current.Value;
            Eat(TokenType.Identifier);
            Eat(TokenType.LParen);
            var parameters = new List<string>();
            if (Current.Type != TokenType.RParen)
            {
                parameters.Add(Current.Value);
                Eat(TokenType.Identifier);
                while (Current.Type == TokenType.Comma)
                {
                    Eat(TokenType.Comma);
                    parameters.Add(Current.Value);
                    Eat(TokenType.Identifier);
                }
            }

            Eat(TokenType.RParen);
            Eat(TokenType.LBrace);
            var body = (BlockNode)Block();
            Eat(TokenType.RBrace);
            return new FuncDefNode(name, parameters, body, Current.Column, Current.Line);
        }
        else if (Current.Type == TokenType.Return)
        {
            Eat(TokenType.Return);
            var val = Expr();
            Eat(TokenType.Semicolon);
            return new ReturnNode(val, Current.Column, Current.Line);
        }
        else if (Current.Type == TokenType.If)
        {
            Eat(TokenType.If);
            Eat(TokenType.LParen);
            var cond = Expr();
            Eat(TokenType.RParen);

            Eat(TokenType.LBrace);
            var thenBody = Block();
            Eat(TokenType.RBrace);

            Node? elseBody = null;

            if (Current.Type == TokenType.Else)
            {
                Eat(TokenType.Else);

                if (Current.Type == TokenType.If) // else if erkannt
                {
                    // rekursiv einen neuen IfNode für else if parsen
                    elseBody = Statement();
                }
                else
                {
                    Eat(TokenType.LBrace);
                    elseBody = Block();
                    Eat(TokenType.RBrace);
                }
            }

            return new IfNode(cond, thenBody, Current.Column, Current.Line, elseBody);
        }

        else if (Current.Type == TokenType.While)
        {
            Eat(TokenType.While);
            Eat(TokenType.LParen);
            var cond = Expr();
            Eat(TokenType.RParen);
            Eat(TokenType.LBrace);
            var body = Block();
            Eat(TokenType.RBrace);
            return new WhileNode(cond, body, Current.Column, Current.Line);
        }
        else if (Current.Type == TokenType.Break)
        {
            Eat(TokenType.Break);
            Eat(TokenType.Semicolon);
            return new BreakNode(Current.Column, Current.Line);
        }
        else if (Current.Type == TokenType.Continue)
        {
            Eat(TokenType.Continue);
            Eat(TokenType.Semicolon);
            return new ContinueNode(Current.Column, Current.Line);
        }
        else if (Current.Type == TokenType.Struct)
        {
            // struct Name { field; field; }
            Eat(TokenType.Struct);
            string structName = Current.Value;
            Eat(TokenType.Identifier);
            Eat(TokenType.LBrace);

            var fields = new List<string>();
            while (Current.Type != TokenType.RBrace)
            {
                if (Current.Type != TokenType.Identifier)
                    throw new Exception($"Expected identifier in struct body, got {Current.Type} at line {Current.Line}, column {Current.Column}.");
                fields.Add(Current.Value);
                Eat(TokenType.Identifier);
                Eat(TokenType.Semicolon);
            }
            Eat(TokenType.RBrace);
            return new StructDefNode(structName, fields, Current.Column, Current.Line);
        }

        // Generische Behandlung für Zuweisungen oder Ausdruck-Statements:
        // Parst eine Primary/Expression-ähnliche linke Seite und prüft auf Assign.
        // Das ermöglicht z.B. p.z = ..., arr[0] = ..., x = ...
        if (Current.Type == TokenType.Identifier || Current.Type == TokenType.LParen || Current.Type == TokenType.LBracket || Current.Type == TokenType.New || Current.Type == TokenType.Number || Current.Type == TokenType.String || Current.Type == TokenType.Boolean || Current.Type == TokenType.Minus)
        {
            var left = Expr();

            // Zuweisung?
            if (Current.Type == TokenType.Assign)
            {
                Eat(TokenType.Assign);
                var right = Expr();
                Eat(TokenType.Semicolon);

                if (left is VarNode || left is ArrayAccessNode || left is MemberAccessNode)
                    return new AssignNode(left, right, Current.Column, Current.Line);

                throw new Exception("Invalid assignment target");
            }

            // Funktionsaufruf als Statement (Expr kann ein FuncCallNode sein)
            if (left is FuncCallNode fc)
            {
                Eat(TokenType.Semicolon);
                return fc;
            }

            // Sonst: Ausdrucksstatement (z. B. standalone Expr ;)
            Eat(TokenType.Semicolon);
            return left;
        }

        // Falls nichts passt -> Fehler
        throw new Exception($"Unexpected token {Current.Type} at line {Current.Line}, column {Current.Column}.");
    }

    private Node Expr() => LogicOr();


    private Node XBitOr()
    {
        var node = XBitXor();
        while (Current.Type == TokenType.BitOr)
        {
            var op = Current.Type;
            Eat(TokenType.BitOr);
            node = new BinOpNode(node, op, XBitXor(), Current.Column, Current.Line);
        }

        return node;
    }

    private Node XBitXor()
    {
        var node = XBitAnd();
        while (Current.Type == TokenType.BitXor)
        {
            var op = Current.Type;
            Eat(TokenType.BitXor);
            node = new BinOpNode(node, op, XBitAnd(), Current.Column, Current.Line);
        }

        return node;
    }

    private Node XBitAnd()
    {
        var node = Equality();
        while (Current.Type == TokenType.BitAnd)
        {
            var op = Current.Type;
            Eat(TokenType.BitAnd);
            node = new BinOpNode(node, op, Equality(), Current.Column, Current.Line);
        }

        return node;
    }


    private Node LogicOr()
    {
        var node = LogicAnd();
        while (Current.Type == TokenType.Or)
        {
            var op = Current.Type;
            Eat(TokenType.Or);
            node = new BinOpNode(node, op, LogicAnd(), Current.Column, Current.Line);
        }

        return node;
    }

    private Node LogicAnd()
    {
        var node = XBitOr();
        while (Current.Type == TokenType.And)
        {
            var op = Current.Type;
            Eat(TokenType.And);
            node = new BinOpNode(node, op, XBitOr(), Current.Column, Current.Line);
        }

        return node;
    }

    private Node Equality()
    {
        var node = Comparison();
        while (Current.Type == TokenType.Equals || Current.Type == TokenType.NotEquals)
        {
            var op = Current.Type;
            Eat(op);
            node = new BinOpNode(node, op, Comparison(), Current.Column, Current.Line);
        }

        return node;
    }

    private Node Comparison()
    {
        var node = BitShift();
        while (Current.Type == TokenType.Less || Current.Type == TokenType.LessEq ||
               Current.Type == TokenType.Greater || Current.Type == TokenType.GreaterEq)
        {
            var op = Current.Type;
            Eat(op);
            node = new BinOpNode(node, op, Term(), Current.Column, Current.Line);
        }

        return node;
    }

    private Node BitShift()
    {
        var node = Term();
        while (Current.Type == TokenType.ShiftLeft || Current.Type == TokenType.ShiftRight)
        {
            var op = Current.Type;
            Eat(op);
            node = new BinOpNode(node, op, Term(), Current.Column, Current.Line);
        }

        return node;
    }

    private Node Term()
    {
        var node = Factor();
        while (Current.Type == TokenType.Plus || Current.Type == TokenType.Minus)
        {
            var op = Current.Type;
            Eat(op);
            node = new BinOpNode(node, op, Factor(), Current.Column, Current.Line);
        }

        return node;
    }

    private Node Factor()
    {
        var node = ExpBin();
        while (Current.Type == TokenType.Mul || Current.Type == TokenType.Div || Current.Type == TokenType.Mod)
        {
            var op = Current.Type;
            Eat(op);
            node = new BinOpNode(node, op, ExpBin(), Current.Column, Current.Line);
        }

        return node;
    }

    private Node ExpBin()
    {
        var node = Unary();
        while (Current.Type == TokenType.Pow)
        {
            Eat(Current.Type);
            node = new BinOpNode(node, TokenType.Pow, Unary(), Current.Column, Current.Line);
        }

        return node;
    }

    private Node Unary()
    {
        if (Current.Type == TokenType.Minus)
        {
            var op = Current.Type;
            Eat(TokenType.Minus);
            return new UnaryOpNode(op, Unary(), Current.Column, Current.Line);
        }

        return Primary();
    }

    private Node Primary()
    {
        // new Struct/Instance als Expression
        if (Current.Type == TokenType.New)
        {
            Eat(TokenType.New);
            string structName = Current.Value;
            Eat(TokenType.Identifier);

            Eat(TokenType.LParen);
            var args = new List<Node>();
            if (Current.Type != TokenType.RParen)
            {
                args.Add(Expr());
                while (Current.Type == TokenType.Comma)
                {
                    Eat(TokenType.Comma);
                    args.Add(Expr());
                }
            }
            Eat(TokenType.RParen);
            return new StructInstanceNode(structName, args, Current.Column, Current.Line);
        }

        if (Current.Type == TokenType.Number)
        {
            var val = double.Parse(Current.Value, CultureInfo.InvariantCulture);
            Eat(TokenType.Number);
            return new NumberNode(val, Current.Column, Current.Line);
        }

        if (Current.Type == TokenType.String)
        {
            var val = Current.Value;
            Eat(TokenType.String);
            return new StringNode(val, Current.Column, Current.Line);
        }

        if (Current.Type == TokenType.Char)
        {
            char val = Current.Value[0];
            Eat(TokenType.Char);
            return new CharNode(val, Current.Column, Current.Line);
        }

        if (Current.Type == TokenType.Boolean)
        {
            var val = Current.Value == "true";
            Eat(TokenType.Boolean);
            return new BoolNode(val, Current.Column, Current.Line);
        }

        if (Current.Type == TokenType.Identifier)
        {
            string name = Current.Value;
            Eat(TokenType.Identifier);

            Node node = new VarNode(name, Current.Column, Current.Line);

            // Funktion aufrufen
            if (Current.Type == TokenType.LParen)
                node = FuncCall(name);

            // Zugriff-Kette: Array-Zugriff oder Memberzugriff (beliebig verschachtelt)
            while (Current.Type == TokenType.LBracket || Current.Type == TokenType.Dot)
            {
                if (Current.Type == TokenType.LBracket)
                {
                    Eat(TokenType.LBracket);
                    var index = Expr();
                    Eat(TokenType.RBracket);
                    node = new ArrayAccessNode(node, index, Current.Column, Current.Line);
                }
                else if (Current.Type == TokenType.Dot)
                {
                    Eat(TokenType.Dot);
                    string member = Current.Value;
                    Eat(TokenType.Identifier);
                    node = new MemberAccessNode(node, member, Current.Column, Current.Line);
                }
            }

            return node;
        }

        if (Current.Type == TokenType.LParen)
        {
            Eat(TokenType.LParen);
            var node = Expr();
            Eat(TokenType.RParen);
            return node;
        }

        if (Current.Type == TokenType.LBracket)
        {
            Eat(TokenType.LBracket);
            var elements = new List<Node>();
            if (Current.Type != TokenType.RBracket)
            {
                elements.Add(Expr());
                while (Current.Type == TokenType.Comma)
                {
                    Eat(TokenType.Comma);
                    elements.Add(Expr());
                }
            }
            Eat(TokenType.RBracket);
            return new ArrayNode(elements, Current.Column, Current.Line);
        }

        throw new Exception($"Unexpected Token {Current.Type} at line {Current.Line}, column {Current.Column}.");
    }

    private FuncCallNode FuncCall(string? name = null)
    {
        if (name == null)
        {
            name = Current.Value;
            Eat(TokenType.Identifier);
        }

        Eat(TokenType.LParen);
        var args = new List<Node>();
        if (Current.Type != TokenType.RParen)
        {
            args.Add(Expr());
            while (Current.Type == TokenType.Comma)
            {
                Eat(TokenType.Comma);
                args.Add(Expr());
            }
        }

        Eat(TokenType.RParen);
        return new FuncCallNode(name, args, Current.Column, Current.Line);
    }
}
