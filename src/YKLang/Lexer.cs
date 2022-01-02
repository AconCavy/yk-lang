namespace YKLang;

public static class Lexer
{
    private static readonly IReadOnlySet<char> s_ignoreCharacters = new HashSet<char> { ' ', '\t', '\r', '\n' };

    private static readonly IReadOnlySet<char> s_separator = new HashSet<char>
    {
        '(', ')', '{', '}', ',', '.', '+', '-', '*', '/', '!', '=', '<', '>', '&', '|', ' ', '\t', '\r', '\n'
    };

    public static IReadOnlyList<Token> Analyze(ReadOnlySpan<char> source)
    {
        var current = 0;
        var tokens = new List<Token>();

        while (current < source.Length)
        {
            if (s_ignoreCharacters.Contains(source[current]))
            {
                current++;
                continue;
            }

            var (type, length) = GetTypeAndLength(source[current..]);
            if (length < 0)
                throw new InvalidOperationException();

            var range = type is TokenType.String
                ? new Range(current + 1, current + length - 1)
                : new Range(current, current + length);

            if (type is TokenType.Other)
            {
                type = SelectKeywordType(source.Slice(current, length));
            }

            tokens.Add(new Token(type, range));
            current += length;
        }

        return tokens;
    }

    private static (TokenType TokenType, int Length) GetTypeAndLength(ReadOnlySpan<char> source)
    {
        return source[0] switch
        {
            '(' => (TokenType.LeftParen, 1),
            ')' => (TokenType.RightParen, 1),
            '{' => (TokenType.LeftBrace, 1),
            '}' => (TokenType.RightBrace, 1),
            ',' => (TokenType.Comma, 1),
            '.' => (TokenType.Dot, 1),
            '+' => (TokenType.Plus, 1),
            '-' => (TokenType.Minus, 1),
            '*' => (TokenType.Multiply, 1),
            '/' => (TokenType.Divide, 1),
            ';' => (TokenType.Semicolon, 1),
            '#' => (TokenType.Hash, CommentLength(source)),
            '!' => IsMatch(source, "!=".AsSpan()) ? (TokenType.NotEqual, 2) : (TokenType.Not, 1),
            '=' => IsMatch(source, "==".AsSpan()) ? (TokenType.Equal, 2) : (TokenType.Assign, 1),
            '<' => IsMatch(source, "<=".AsSpan()) ? (TokenType.LessEqual, 2) : (TokenType.Less, 1),
            '>' => IsMatch(source, ">=".AsSpan()) ? (TokenType.GreaterEqual, 2) : (TokenType.Greater, 1),
            '"' => (TokenType.String, StringLength(source)),
            >= '0' and <= '9' => (TokenType.Number, NumberLength(source)),
            '&' => IsMatch(source, "&&".AsSpan()) ? (TokenType.And, 2) : (TokenType.BitwiseAnd, 1),
            '|' => IsMatch(source, "||".AsSpan()) ? (TokenType.Or, 2) : (TokenType.BitwiseOr, 1),
            _ => (TokenType.Other, OtherLength(source))
        };
    }

    public static bool IsMatch(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
    {
        if (!source.StartsWith(target))
            return false;
        return source.Length == target.Length || s_separator.Contains(source[target.Length]);
    }

    public static int CommentLength(ReadOnlySpan<char> source)
    {
        for (var i = 0; i < source.Length; i++)
        {
            if (source[i] == '\n')
                return i + 1;
        }

        return source.Length;
    }

    public static int StringLength(ReadOnlySpan<char> source)
    {
        for (var i = 1; i < source.Length; i++)
        {
            if (source[i] == '"')
                return i + 1;
        }

        return -1;
    }

    public static int NumberLength(ReadOnlySpan<char> source)
    {
        var dot = false;
        for (var i = 0; i < source.Length; i++)
        {
            if (char.IsDigit(source[i]))
                continue;
            if (source[i] == '.')
            {
                if (!dot)
                    dot = true;
                else if (i + 1 < source.Length && char.IsDigit(source[i + 1]))
                    return -1;
                else
                    return i;
            }
            else
            {
                return i;
            }
        }

        return source.Length;
    }

    public static int OtherLength(ReadOnlySpan<char> source)
    {
        for (var i = 0; i < source.Length; i++)
        {
            if (s_separator.Contains(source[i]))
                return i;
        }

        return source.Length;
    }

    private static TokenType SelectKeywordType(ReadOnlySpan<char> source)
    {
        if (source.Equals("class".AsSpan(), StringComparison.Ordinal))
            return TokenType.Class;
        if (source.Equals("if".AsSpan(), StringComparison.Ordinal))
            return TokenType.If;
        if (source.Equals("else".AsSpan(), StringComparison.Ordinal))
            return TokenType.Else;
        if (source.Equals("while".AsSpan(), StringComparison.Ordinal))
            return TokenType.While;
        if (source.Equals("for".AsSpan(), StringComparison.Ordinal))
            return TokenType.For;
        if (source.Equals("nil".AsSpan(), StringComparison.Ordinal))
            return TokenType.Nil;
        if (source.Equals("return".AsSpan(), StringComparison.Ordinal))
            return TokenType.Return;
        if (source.Equals("this".AsSpan(), StringComparison.Ordinal))
            return TokenType.This;
        if (source.Equals("base".AsSpan(), StringComparison.Ordinal))
            return TokenType.Base;
        if (source.Equals("var".AsSpan(), StringComparison.Ordinal))
            return TokenType.Var;
        if (source.Equals("function".AsSpan(), StringComparison.Ordinal))
            return TokenType.Function;
        if (source.Equals("true".AsSpan(), StringComparison.Ordinal))
            return TokenType.True;
        if (source.Equals("false".AsSpan(), StringComparison.Ordinal))
            return TokenType.False;
        return TokenType.Identifier;
    }
}
