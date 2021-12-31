namespace YKLang.Statements;

public class Variable : Statement
{
    public Token Name { get; }
    public Expressions.Expression Initializer { get; }

    public Variable(Token name, Expressions.Expression initializer)
    {
        Name = name;
        Initializer = initializer;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
