namespace YKLang.Expressions;

public interface IVisitor<out T>
{
    T Visit(Assign expression);
    T Visit(Binary expression);
    T Visit(Call expression);
    T Visit(Get expression);
    T Visit(Grouping expression);
    T Visit(Literal expression);
    T Visit(Logical expression);
    T Visit(Set expression);
    T Visit(Base expression);
    T Visit(This expression);
    T Visit(Unary expression);
    T Visit(Variable expression);
}
