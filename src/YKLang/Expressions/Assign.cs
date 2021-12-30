namespace YKLang.Expressions;

internal class Assign : Expression
{
    internal Token Name { get; }
    internal Expression Value { get; }

    internal Assign(Token name, Expression value)
    {
        Name = name;
        Value = value;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
