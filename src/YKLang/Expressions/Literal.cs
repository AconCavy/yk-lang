namespace YKLang.Expressions;

internal class Literal : Expression
{
    internal Object Value { get; }

    internal Literal(Object value)
    {
        Value = value;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
