namespace YKLang.Expressions;

internal class This : Expression
{
    internal Token Keyword { get; }

    internal This(Token keyword)
    {
        Keyword = keyword;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
