namespace YKLang.Statements;

internal interface IVisitor<out T>
{
    internal T Visit(Block statement);
    internal T Visit(Class statement);
    internal T Visit(Expression statement);
    internal T Visit(Function statement);
    internal T Visit(If statement);
    internal T Visit(Return statement);
    internal T Visit(Variable statement);
    internal T Visit(While statement);
}
