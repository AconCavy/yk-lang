namespace YKLang.Expressions;

internal abstract class Expression
{
    internal abstract T Accept<T>(IVisitor<T> visitor);
}
