namespace YKLang.Statements;

internal class Expression : Statement
{
    private Expressions.Expression Body { get; }

    internal Expression(Expressions.Expression body)
    {
        Body = body;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
