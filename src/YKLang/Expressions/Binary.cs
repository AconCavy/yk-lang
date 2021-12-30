namespace YKLang.Expressions;

internal class Binary : Expression
{
    internal Expression Left { get; }
    internal Token Operator { get; }
    internal Expression Right { get; }

    internal Binary(Expression left, Token @operator, Expression right)
    {
        Left = left;
        Right = right;
        Operator = @operator;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
