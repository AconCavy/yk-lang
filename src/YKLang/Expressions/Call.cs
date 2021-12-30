namespace YKLang.Expressions;

internal class Call : Expression
{
    internal Expression Callee { get; }
    internal Token Paren { get; }
    internal Expression[] Arguments { get; }

    internal Call(Expression callee, Token paren, IEnumerable<Expression> arguments)
    {
        Callee = callee;
        Paren = paren;
        Arguments = arguments as Expression[] ?? arguments.ToArray();
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
