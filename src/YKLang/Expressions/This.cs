namespace YKLang.Expressions;

public class This : Expression
{
    public Token Keyword { get; }

    public This(Token keyword)
    {
        Keyword = keyword;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
