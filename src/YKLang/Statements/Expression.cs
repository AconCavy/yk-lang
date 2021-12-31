namespace YKLang.Statements;

public class Expression : Statement
{
    public Expressions.Expression Body { get; }

    public Expression(Expressions.Expression body)
    {
        Body = body;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
