namespace YKLang.Statements;

public class Function : Statement
{
    public Token Name { get; }
    public Token[] Params { get; }
    public Statement[] Body { get; }

    public Function(Token name, IEnumerable<Token> @params, IEnumerable<Statement> body)
    {
        Name = name;
        Params = @params.ToArray();
        Body = body.ToArray();
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
