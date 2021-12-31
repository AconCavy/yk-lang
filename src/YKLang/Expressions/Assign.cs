namespace YKLang.Expressions;

public class Assign : Expression
{
    public Token Name { get; }
    public Expression Value { get; }

    public Assign(Token name, Expression value)
    {
        Name = name;
        Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
