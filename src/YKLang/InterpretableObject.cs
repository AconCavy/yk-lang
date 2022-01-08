using YKLang.Statements;

namespace YKLang;

public class InterpretableObject
{
    public string Source { get; }
    public IReadOnlyList<Statement> Statements { get; }

    public InterpretableObject(string source, IReadOnlyList<Statement> statements)
    {
        Source = source;
        Statements = statements;
    }
}
