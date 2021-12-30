namespace YKLang.Statements;

internal class Return : Statement
{
    internal Token Keyword { get; }
    internal Expressions.Expression Value { get; }

    internal Return(Token keyword, Expressions.Expression value)
    {
        Keyword = keyword;
        Value = value;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
