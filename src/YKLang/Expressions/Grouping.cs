namespace YKLang.Expressions;

internal class Grouping : Expression
{
    internal Expression Expression { get; }

    internal Grouping(Expression expression)
    {
        Expression = expression;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
