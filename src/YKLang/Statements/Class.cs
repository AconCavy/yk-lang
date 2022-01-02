namespace YKLang.Statements;

public class Class : Statement
{
    public Token Name { get; }
    public Expressions.Variable? Base { get; }
    public Function[] Functions { get; }

    public Class(Token name, Expressions.Variable? @base, IEnumerable<Function> functions)
    {
        Name = name;
        Base = @base;
        Functions = functions as Function[] ?? functions.ToArray();
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
