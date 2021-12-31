namespace YKLang.Expressions;

public class Call : Expression
{
    public Expression Callee { get; }
    public Token Paren { get; }
    public Expression[] Arguments { get; }

    public Call(Expression callee, Token paren, IEnumerable<Expression> arguments)
    {
        Callee = callee;
        Paren = paren;
        Arguments = arguments as Expression[] ?? arguments.ToArray();
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
