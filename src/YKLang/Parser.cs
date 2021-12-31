using YKLang.Expressions;

namespace YKLang;

public class Parser
{
    private string Source { get; }
    private Token[] Tokens { get; }
    private int Index { get; set; }
    private Token Current => Tokens[Index];

    private readonly TokenType[] _equalityTokens = { TokenType.Equal, TokenType.NotEqual };
    private readonly TokenType[] _comparisonTokens =
    {
        TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual
    };
    private readonly TokenType[] _termTokens = { TokenType.Plus, TokenType.Minus };
    private readonly TokenType[] _factorTokens = { TokenType.Multiply, TokenType.Divide };
    private readonly TokenType[] _unaryTokens = { TokenType.Not, TokenType.Minus };

    public Parser(string source, IEnumerable<Token> tokens)
    {
        Source = source;
        Tokens = tokens as Token[] ?? tokens.ToArray();
        Index = 0;
    }

    public Expression Expression()
    {
        return Equality();
    }

    private Expression Equality()
    {
        var expression = Comparison();
        while (IsMatch(_equalityTokens))
        {
            var op = Current;
            Index++;
            var right = Comparison();
            expression = new Binary(expression, op, right);
        }

        return expression;
    }

    private Expression Comparison()
    {
        var expression = Term();
        while (IsMatch(_comparisonTokens))
        {
            var op = Current;
            Index++;
            var right = Term();
            expression = new Binary(expression, op, right);
        }

        return expression;
    }

    private Expression Term()
    {
        var expression = Factor();
        while (IsMatch(_termTokens))
        {
            var op = Current;
            Index++;
            var right = Factor();
            expression = new Binary(expression, op, right);
        }

        return expression;
    }

    private Expression Factor()
    {
        var expression = Unary();
        while (IsMatch(_factorTokens))
        {
            var op = Current;
            Index++;
            var right = Unary();
            expression = new Binary(expression, op, right);
        }

        return expression;
    }

    private Expression Unary()
    {
        if (!IsMatch(_unaryTokens))
            return Primary();

        var op = Current;
        Index++;
        var right = Unary();
        return new Unary(op, right);
    }

    private Expression Primary()
    {
        switch (Current.Type)
        {
            case TokenType.True:
                return new Literal(true);
            case TokenType.False:
                return new Literal(false);
            case TokenType.Nil:
                return new Literal(null);
            case TokenType.Number:
                return new Literal(double.Parse(Source[Current.Range]));
            case TokenType.String:
                return new Literal(Source[Current.Range]);
            default: throw new ParseException();
        }
    }

    private bool IsMatch(ReadOnlySpan<TokenType> types)
    {
        if (Index < 0 || Tokens.Length <= Index)
            return false;
        foreach (var type in types)
        {
            if (type == Current.Type)
                return true;
        }

        return false;
    }
}
