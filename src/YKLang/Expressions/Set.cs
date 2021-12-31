namespace YKLang.Expressions;

public class Set : Expression
{
    public Expression Object { get; }
    public Token Name { get; }
    public Expression Value { get; }

    public Set(Expression @object, Token name, Expression value)
    {
        Object = @object;
        Name = name;
        Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
