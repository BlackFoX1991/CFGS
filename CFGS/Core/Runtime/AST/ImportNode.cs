namespace CFGS.Core.Runtime.AST;

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
