namespace YKLang.Expressions;

internal class Unary : Expression
{
    internal Token Operator { get; }
    internal Expression Right { get; }

    internal Unary(Token @operator, Expression right)
    {
        Operator = @operator;
        Right = right;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
