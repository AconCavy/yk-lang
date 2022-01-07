using YKLang.Statements;

namespace YKLang;

public class InterpretableObject
{
    public string Source { get; }
    public Statement[] Statements { get; }

    public InterpretableObject(string source, IEnumerable<Statement> statements)
    {
        Source = source;
        Statements = statements as Statement[] ?? statements.ToArray();
    }
}
