namespace YKLang.Expressions;

public class Get : Expression
{
    public Expression Object { get; }
    public Token Name { get; }

    public Get(Expression @object, Token name)
    {
        Object = @object;
        Name = name;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
