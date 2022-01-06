namespace YKLang.Statements;

public class While : Statement
{
    public Expressions.Expression Condition { get; }
    public Statement Body { get; }

    public While(Expressions.Expression condition, Statement body)
    {
        Condition = condition;
        Body = body;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
