namespace YKLang.Statements;

internal class Function : Statement
{
    internal Token Name { get; }
    internal Token[]? Params { get; }
    internal Statement[]? Body { get; }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
