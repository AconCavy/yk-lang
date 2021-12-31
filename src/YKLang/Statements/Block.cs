namespace YKLang.Statements;

public class Block : Statement
{
    public Statement[] Statements { get; }

    public Block(IEnumerable<Statement> statements)
    {
        Statements = statements as Statement[] ?? statements.ToArray();
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
