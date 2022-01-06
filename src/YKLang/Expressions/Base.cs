namespace YKLang.Expressions;

public class Base : Expression
{
    public Token Keyword { get; }
    public Token Method { get; }

    public Base(Token keyword, Token method)
    {
        Keyword = keyword;
        Method = method;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
