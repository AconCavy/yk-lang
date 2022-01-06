namespace YKLang.Statements;

public abstract class Statement
{
    public abstract T Accept<T>(IVisitor<T> visitor);
}
