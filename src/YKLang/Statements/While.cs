namespace YKLang.Statements;

internal class While : Statement
{
    internal Expressions.Expression Condition { get; }
    internal Statement Body { get; }

    internal While(Expressions.Expression condition, Statement body)
    {
        Condition = condition;
        Body = body;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
