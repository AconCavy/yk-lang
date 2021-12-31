namespace YKLang.Statements;

public interface IVisitor<out T>
{
    public T Visit(Block statement);
    public T Visit(Class statement);
    public T Visit(Expression statement);
    public T Visit(Function statement);
    public T Visit(If statement);
    public T Visit(Return statement);
    public T Visit(Variable statement);
    public T Visit(While statement);
}
