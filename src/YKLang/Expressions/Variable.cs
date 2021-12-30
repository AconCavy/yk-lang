namespace YKLang.Expressions;

internal class Variable : Expression
{
    internal Token Name { get; }

    internal Variable(Token name)
    {
        Name = name;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
