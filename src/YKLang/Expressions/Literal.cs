namespace YKLang.Expressions;

public class Literal : Expression
{
    public dynamic? Value { get; }

    public Literal(dynamic? value)
    {
        Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
