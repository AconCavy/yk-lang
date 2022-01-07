namespace YKLang;

public class ParsableObject
{
    public string Source { get; }
    public IReadOnlyList<Token> Tokens { get; }

    public ParsableObject(string source)
    {
        Source = source;
        Tokens = Lexer.Analyze(Source);
    }
}
