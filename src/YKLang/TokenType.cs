namespace YKLang;

public enum TokenType
{
    // Single characters
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    Comma,
    Dot,
    Plus,
    Minus,
    Star,
    Slash,
    Semicolon,

    // Single or Double characters
    Bang,
    BangEqual,
    Assign,
    Equal,
    Greater,
    GreaterEqual,
    Less,
    LessEqual,

    // Literal
    Identifier,
    String,
    Number,

    // Keywords
    And,
    Or,
    Class,
    If,
    Else,
    While,
    For,
    Nil,
    Return,
    This,
    Base,
    Var,
    Function,

    // Special
    Eof
}
