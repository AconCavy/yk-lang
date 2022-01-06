namespace YKLang.Statements;

public class Class : Statement
{
    public Token Name { get; }
    public Expressions.Variable? BaseClass { get; }
    public Function[] Methods { get; }

    public Class(Token name, Expressions.Variable? baseClass, IEnumerable<Function> methods)
    {
        Name = name;
        BaseClass = baseClass;
        Methods = methods as Function[] ?? methods.ToArray();
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
