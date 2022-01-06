namespace YKLang.Statements;

public class Function : Statement
{
    public Token Name { get; }
    public Token[] Parameters { get; }
    public Statement[] Body { get; }

    public Function(Token name, IEnumerable<Token> parameters, IEnumerable<Statement> body)
    {
        Name = name;
        Parameters = parameters.ToArray();
        Body = body.ToArray();
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
