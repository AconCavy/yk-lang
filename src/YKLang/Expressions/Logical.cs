namespace YKLang.Expressions;

public class Logical : Expression
{
    public Expression Left { get; }
    public Token Operator { get; }
    public Expression Right { get; }

    public Logical(Expression left, Token @operator, Expression right)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
