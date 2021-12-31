namespace YKLang.Expressions;

internal class Literal : Expression
{
    internal dynamic? Value { get; }

    internal Literal(dynamic? value)
    {
        Value = value;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
