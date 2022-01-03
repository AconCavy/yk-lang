namespace YKLang.Statements;

public class Class : Statement
{
    public Token Name { get; }
    public Expressions.Variable? Base { get; }
    public Function[] Methods { get; }

    public Class(Token name, Expressions.Variable? @base, IEnumerable<Function> methods)
    {
        Name = name;
        Base = @base;
        Methods = methods as Function[] ?? methods.ToArray();
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
