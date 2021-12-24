namespace YKLang;

public class Token
{
    public TokenType Type { get; }
    public string Value { get; }
    public Token(TokenType type, string value) => (Type, Value) = (type, value);
}
