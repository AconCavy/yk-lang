namespace YKLang.Expressions;

public class Unary : Expression
{
    public Token Operator { get; }
    public Expression Right { get; }

    public Unary(Token @operator, Expression right)
    {
        Operator = @operator;
        Right = right;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
