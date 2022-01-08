namespace YKLang;

public readonly record struct Token(TokenType Type, Range Range)
{
    public static Token Eof => new(TokenType.Eof, default);
}
