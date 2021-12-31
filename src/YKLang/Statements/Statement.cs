namespace YKLang.Statements;

public abstract partial class Statement
{
    public abstract T Accept<T>(IVisitor<T> visitor);
}
