namespace YKLang.Expressions;

internal class Logical : Expression
{
    internal Expression Left { get; }
    internal Token Operator { get; }
    internal Expression Right { get; }

    internal Logical(Expression left, Token @operator, Expression right)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
