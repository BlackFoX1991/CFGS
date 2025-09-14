namespace CFGS.Core.Analytics;

#pragma warning disable CS8602
public class Lexer(string text)
{
    private int _pos;
    private int _line = 1;
    private int _column = 1;
    private char Current => _pos < text.Length ? text[_pos] : '\0';

    private readonly Dictionary<string, TokenType> _keywords = new()
    {
        { "if", TokenType.If },
        {"else",TokenType.Else}, 
        { "while", TokenType.While }, 
        { "print", TokenType.Print },
        { "printc", TokenType.Printc },
        { "import", TokenType.Import },
        { "func", TokenType.Func }, 
        { "return", TokenType.Return },
        { "true", TokenType.Boolean }, 
        { "false", TokenType.Boolean },
        { "struct", TokenType.Struct },
        { "new", TokenType.New },
        {"break", TokenType.Break },
        {"continue",TokenType.Continue },
        {"try", TokenType.Try },
        {"catch", TokenType.Catch },
        {"finally", TokenType.Finally },
        {"throw", TokenType.Throw },
        {"match", TokenType.Match },
        {"case", TokenType.Case },
        {"default", TokenType.Default },
        {"delete",TokenType.Delete},
        {"enum",TokenType.Enum}
    };

    private void Advance()
    {
        if (Current == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }
        _pos++;
    }

    private void SkipWhitespace()
    {
        while (char.IsWhiteSpace(Current)) Advance();
    }

    private string Number()
    {
        string result = "";
        while (char.IsDigit(Current) || Current == '.')
        {
            result += Current;
            Advance();
        }

        return result;
    }

    private string Identifier()
    {
        string result = "";
        while (char.IsLetterOrDigit(Current) || Current == '_')
        {
            result += Current;
            Advance();
        }

        return result;
    }

    private string StringLiteral(char separator = '"')
    {
        Advance(); // skip opening quote
        string result = "";
        while (Current != separator && Current != '\0')
        {
            if (Current == '\\')
            {
                Advance();
                result += Current switch
                {
                    'n' => '\n',
                    'r' => '\r',
                    '0' => '\0',
                    't' => '\t',
                    '"' => '"',
                    '\'' => '\'',
                    '\\' => '\\',
                    _ => Current
                };
            }
            else
            {
                result += Current;
            }

            Advance();
        }

        Advance(); // skip closing quote
        return result;
    }

    public List<Token> GetTokens()
    {
        var tokens = new List<Token>();
        while (Current != '\0')
        {
            SkipWhitespace();
            if (char.IsDigit(Current)) tokens.Add(new Token(TokenType.Number, Number()));
            else if (char.IsLetter(Current))
            {
                var id = Identifier();
                tokens.Add(new Token(_keywords.GetValueOrDefault(id, TokenType.Identifier), id, _line, _column));
            }
            else if (Current == '"') tokens.Add(new Token(TokenType.String, StringLiteral(), _line, _column));
            else if (Current == '\'') tokens.Add(new Token(TokenType.Char, StringLiteral('\''), _line, _column));
            else
            {
                switch (Current)
                {
                    case '#': // skip comment
                        while (Current != '\0' && Current != '\n')
                            Advance();
                        if (Current == '\n') Advance();
                        break;
                    case '+':
                        Advance();
                        if (Current == '+')
                        {
                            Advance();
                            tokens.Add(new Token(TokenType.PlusPlus, "", _line, _column));
                        }
                        else tokens.Add(new Token(TokenType.Plus, "", _line, _column));
                        break;
                    case '-':
                        Advance();
                        if (Current == '-')
                        {
                            Advance();
                            tokens.Add(new Token(TokenType.MinusMinus, "", _line, _column));
                        }
                        else tokens.Add(new Token(TokenType.Minus, "", _line, _column));
                        break;
                    case '^':
                        tokens.Add(new Token(TokenType.BitXor, "", _line, _column));
                        Advance();
                        break;
                    case '*':
                        Advance();
                        if (Current == '*')
                        {
                            Advance();
                            tokens.Add(new Token(TokenType.Pow, "", _line, _column));
                        }
                        else tokens.Add(new Token(TokenType.Mul, "", _line, _column));

                        break;
                    case '/':
                        tokens.Add(new Token(TokenType.Div, "", _line, _column));
                        Advance();
                        break;

                    case '%':
                        tokens.Add(new Token(TokenType.Mod, "", _line, _column));
                        Advance();
                        break;
                    case '=':
                        Advance();
                        if (Current == '=')
                        {
                            Advance();
                            tokens.Add(new Token(TokenType.Equals, "", _line, _column));
                        }
                        else tokens.Add(new Token(TokenType.Assign, "", _line, _column));

                        break;
                    case '!':
                        Advance();
                        if (Current == '=')
                        {
                            Advance();
                            tokens.Add(new Token(TokenType.NotEquals, "", _line, _column));
                        }
                        else
                            tokens.Add(new Token(TokenType.Not, "", _line, _column));
                        break;
                    case '<':
                        Advance();
                        if (Current == '=')
                        {
                            Advance();
                            tokens.Add(new Token(TokenType.LessEq, "", _line, _column));
                        }
                        else if (Current == '<')
                        {
                            Advance();
                            tokens.Add(new Token(TokenType.ShiftLeft, "", _line, _column));
                        }
                        else tokens.Add(new Token(TokenType.Less, "", _line, _column));

                        break;
                    case '>':
                        Advance();
                        if (Current == '=')
                        {
                            Advance();
                            tokens.Add(new Token(TokenType.GreaterEq, "", _line, _column));

                        }
                        else if (Current == '>')
                        {
                            Advance();
                            tokens.Add(new Token(TokenType.ShiftRight, "", _line, _column));
                        }
                        else tokens.Add(new Token(TokenType.Greater, "", _line, _column));

                        break;
                    case '&':
                        Advance();
                        if (Current == '&')
                        {
                            Advance();
                            tokens.Add(new Token(TokenType.And, "", _line, _column));
                        }
                        else tokens.Add(new Token(TokenType.BitAnd, "", _line, _column));

                        break;
                    case '|':
                        Advance();
                        if (Current == '|')
                        {
                            Advance();
                            tokens.Add(new Token(TokenType.Or, "", _line, _column));
                        }
                        else tokens.Add(new Token(TokenType.BitOr, "", _line, _column));
                        break;

                    case '(':
                        tokens.Add(new Token(TokenType.LParen, "", _line, _column));
                        Advance();
                        break;
                    case ')':
                        tokens.Add(new Token(TokenType.RParen, "", _line, _column));
                        Advance();
                        break;
                    case '{':
                        tokens.Add(new Token(TokenType.LBrace, "", _line, _column));
                        Advance();
                        break;
                    case '}':
                        tokens.Add(new Token(TokenType.RBrace, "", _line, _column));
                        Advance();
                        break;
                    case ';':
                        tokens.Add(new Token(TokenType.Semicolon, "", _line, _column));
                        Advance();
                        break;
                    case ',':
                        tokens.Add(new Token(TokenType.Comma, "", _line, _column));
                        Advance();
                        break;
                    case '.':
                        tokens.Add(new Token(TokenType.Dot, ".", _line, _column));
                        Advance();
                        break;
                    case '[':
                        tokens.Add(new Token(TokenType.LBracket, "", _line, _column));
                        Advance();
                        break;
                    case ']':
                        tokens.Add(new Token(TokenType.RBracket, "", _line, _column));
                        Advance();
                        break;
                    case ':':
                        tokens.Add(new Token(TokenType.Colon, "", _line, _column));
                        Advance();
                        break;
                    default: Advance(); break;
                }
            }
        }

        tokens.Add(new Token(TokenType.Eof));
        return tokens;
    }
}
