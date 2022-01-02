namespace YKLang;

public interface IAstBuilder<out T> : Statements.IVisitor<T>, Expressions.IVisitor<T>
{
}
