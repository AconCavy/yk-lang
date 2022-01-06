namespace YKLang.Expressions;

public class Variable : Expression
{
    public Token Name { get; }

    public Variable(Token name)
    {
        Name = name;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
