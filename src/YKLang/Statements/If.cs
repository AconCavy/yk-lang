namespace YKLang.Statements;

public class If : Statement
{
    public Expressions.Expression Condition { get; }
    public Statement ThenBranch { get; }
    public Statement? ElseBranch { get; }

    public If(Expressions.Expression condition, Statement thenBranch, Statement? elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
