namespace YKLang.Expressions;

public class Grouping : Expression
{
    public Expression Expression { get; }

    public Grouping(Expression expression)
    {
        Expression = expression;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
