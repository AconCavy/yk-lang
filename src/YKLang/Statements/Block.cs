namespace YKLang.Statements;

internal class Block : Statement
{
    internal Statement[] Statements { get; }

    internal Block(IEnumerable<Statement> statements)
    {
        Statements = statements as Statement[] ?? statements.ToArray();
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
