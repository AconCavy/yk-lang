namespace YKLang;

public class ParsableObject
{
    public string Source { get; }
    public IReadOnlyList<Token> Tokens { get; }

    public ParsableObject(string source, IReadOnlyList<Token> tokens)
    {
        Source = source;
        Tokens = tokens;
    }
}
