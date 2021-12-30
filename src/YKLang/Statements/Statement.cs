namespace YKLang.Statements;

internal abstract partial class Statement
{
    internal abstract T Accept<T>(IVisitor<T> visitor);
}
