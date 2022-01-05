namespace YKLang;

public readonly record struct Token(TokenType Type, Range Range)
{
    public static Token Eof => new Token(TokenType.Eof, default);
}
