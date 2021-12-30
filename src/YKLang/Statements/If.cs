namespace YKLang.Statements;

internal class If : Statement
{
    internal Expressions.Expression Condition { get; }
    internal Statement Then { get; }
    internal Statement Else { get; }

    internal If(Expressions.Expression condition, Statement then, Statement @else)
    {
        Condition = condition;
        Then = then;
        Else = @else;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
