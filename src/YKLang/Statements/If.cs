namespace YKLang.Statements;

public class If : Statement
{
    public Expressions.Expression Condition { get; }
    public Statement Then { get; }
    public Statement Else { get; }

    public If(Expressions.Expression condition, Statement then, Statement @else)
    {
        Condition = condition;
        Then = then;
        Else = @else;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
