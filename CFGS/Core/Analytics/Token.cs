namespace CFGS.Core.Analytics;

#pragma warning disable CS8602
public class Token(TokenType type, string value = "", int line = 0, int column = 0)
{
    public TokenType Type { get; } = type;
    public string Value { get; } = value;
    public int Line { get; } = line;
    public int Column { get; } = column;

    public override string ToString() => $"{Type}({Value}) at {Line}:{Column}";
}
