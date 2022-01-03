namespace YKLang;

public enum TokenType
{
    None,
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    Comma,
    Dot,
    Plus,
    Minus,
    Multiply,
    Divide,
    Colon,
    Semicolon,
    Hash,

    Assign,
    Not,
    Equal,
    NotEqual,
    Greater,
    GreaterEqual,
    Less,
    LessEqual,

    Identifier,
    String,
    Number,
    True,
    False,

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

    Other
}
