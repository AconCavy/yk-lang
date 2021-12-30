namespace YKLang.Expressions;

internal class Set : Expression
{
    internal Expression Object { get; }
    internal Token Name { get; }
    internal Expression Value { get; }

    internal Set(Expression @object, Token name, Expression value)
    {
        Object = @object;
        Name = name;
        Value = value;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
