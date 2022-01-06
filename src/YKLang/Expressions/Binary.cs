namespace YKLang.Expressions;

public class Binary : Expression
{
    public Expression Left { get; }
    public Token Operator { get; }
    public Expression Right { get; }

    public Binary(Expression left, Token @operator, Expression right)
    {
        Left = left;
        Right = right;
        Operator = @operator;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
