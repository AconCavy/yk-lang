namespace YKLang.Expressions;

internal interface IVisitor<out T>
{
    internal T Visit(Assign expression);
    internal T Visit(Binary expression);
    internal T Visit(Call expression);
    internal T Visit(Get expression);
    internal T Visit(Grouping expression);
    internal T Visit(Literal expression);
    internal T Visit(Logical expression);
    internal T Visit(Set expression);
    internal T Visit(Base expression);
    internal T Visit(This expression);
    internal T Visit(Unary expression);
    internal T Visit(Variable expression);
}
