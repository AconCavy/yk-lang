namespace YKLang.Statements;

internal class Variable : Statement
{
    internal Token Name { get; }
    internal Expressions.Expression Initializer { get; }

    internal Variable(Token name, Expressions.Expression initializer)
    {
        Name = name;
        Initializer = initializer;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
