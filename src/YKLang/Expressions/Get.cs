namespace YKLang.Expressions;

internal class Get : Expression
{
    internal Expression Object { get; }
    internal Token Name { get; }

    internal Get(Expression @object, Token name)
    {
        Object = @object;
        Name = name;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
