namespace CFGS.Core.Runtime.cException;

#pragma warning disable CS8602
public class ReturnException(object? value) : System.Exception
{
    public readonly object? Value = value;
}