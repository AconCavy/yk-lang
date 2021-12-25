namespace YKLang;

public static class Lexer
{
    private static readonly IReadOnlySet<char> s_ignoreCharacters = new HashSet<char> { ' ', '\t', '\r', '\n' };

    private static readonly IReadOnlySet<char> s_separator = new HashSet<char>
    {
        '(', ')', '{', '}', ',', '.', '+', '-', '*', '/', '!', '=', '<', '>', '&', '|', ' ', '\t', '\r', '\n'
    };

    private static readonly IReadOnlyDictionary<string, TokenType> s_keywords = new Dictionary<string, TokenType>
    {
        { "class", TokenType.Class },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "while", TokenType.While },
        { "for", TokenType.For },
        { "nil", TokenType.Nil },
        { "return", TokenType.Return },
        { "this", TokenType.This },
        { "base", TokenType.Base },
        { "var", TokenType.Var },
        { "function", TokenType.Function }
    };

    public static ICollection<Token> Analyze(ReadOnlySpan<char> source)
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
                var literal = source.Slice(current, length).ToString();
                type = s_keywords.ContainsKey(literal) ? s_keywords[literal] : TokenType.Identifier;
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
            '*' => (TokenType.Star, 1),
            '/' => (TokenType.Slash, 1),
            ';' => (TokenType.Semicolon, 1),
            '#' => (TokenType.Hash, CommentLength(source)),
            '!' => IsMatch(source, "!=") ? (TokenType.BangEqual, 2) : (TokenType.Bang, 1),
            '=' => IsMatch(source, "==") ? (TokenType.Equal, 2) : (TokenType.Assign, 1),
            '<' => IsMatch(source, "<=") ? (TokenType.LessEqual, 2) : (TokenType.Less, 1),
            '>' => IsMatch(source, ">=") ? (TokenType.GreaterEqual, 2) : (TokenType.Greater, 1),
            '"' => (TokenType.String, StringLength(source)),
            >= '0' and <= '9' => (TokenType.Number, NumberLength(source)),
            '&' => IsMatch(source, "&&") ? (TokenType.And, 2) : (TokenType.BitwiseAnd, 1),
            '|' => IsMatch(source, "||") ? (TokenType.Or, 2) : (TokenType.BitwiseOr, 1),
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
}
