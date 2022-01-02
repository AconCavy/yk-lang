namespace YKLang.Statements;

public class Return : Statement
{
    public Token Keyword { get; }
    public Expressions.Expression? Value { get; }

    public Return(Token keyword, Expressions.Expression? value)
    {
        Keyword = keyword;
        Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
