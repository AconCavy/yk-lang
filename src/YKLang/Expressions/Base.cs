namespace YKLang.Expressions;

internal class Base : Expression
{
    internal Token Keyword { get; }
    internal Token Method { get; }

    internal Base(Token keyword, Token method)
    {
        Keyword = keyword;
        Method = method;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
