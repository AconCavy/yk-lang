namespace YKLang;

public enum TokenType
{
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
    Hash,

    Bang,
    BangEqual,
    Assign,
    Equal,
    Greater,
    GreaterEqual,
    Less,
    LessEqual,

    Identifier,
    String,
    Number,

    And,
    Or,
    BitwiseAnd,
    BitwiseOr,

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

    Eof,
    Other
}
