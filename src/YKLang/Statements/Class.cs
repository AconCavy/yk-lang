namespace YKLang.Statements;

internal class Class : Statement
{
    internal Token Name { get; }
    internal Expressions.Variable Base { get; }
    internal Function[]? Functions { get; }

    internal Class(Token name, Expressions.Variable @base, IEnumerable<Function> functions)
    {
        Name = name;
        Base = @base;
        Functions = functions as Function[] ?? functions.ToArray();
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.Visit(this);
    }
}
