namespace YKLang.Statements;

public class Function : Statement
{
    public Token Name { get; }
    public Token[]? Params { get; }
    public Statement[]? Body { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
