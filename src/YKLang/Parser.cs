using YKLang.Expressions;

namespace YKLang;

public static class Parser
{
    public static Expression Parse(string source, IReadOnlyList<Token> tokens)
    {
        var equalityTokens = new[] { TokenType.Equal, TokenType.NotEqual };
        var comparisonTokens = new[] { TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual };
        var termTokens = new[] { TokenType.Plus, TokenType.Minus };
        var factorTokens = new[] { TokenType.Multiply, TokenType.Divide };
        var unaryTokens = new[] { TokenType.Not, TokenType.Minus };

        var current = 0;

        bool IsSafeIndex() => 0 <= current && current <= tokens.Count;
        bool IsMatch(IEnumerable<TokenType> types) => IsSafeIndex() && types.Contains(tokens[current].Type);

        Expression Expression()
        {
            return Equality();
        }

        Expression Equality()
        {
            var expression = Comparison();
            while (IsMatch(equalityTokens))
            {
                var op = tokens[current++];
                var right = Comparison();
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        Expression Comparison()
        {
            var expression = Term();
            while (IsMatch(comparisonTokens))
            {
                var op = tokens[current++];
                var right = Term();
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        Expression Term()
        {
            var expression = Factor();
            while (IsMatch(termTokens))
            {
                var op = tokens[current++];
                var right = Factor();
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        Expression Factor()
        {
            var expression = Unary();
            while (IsMatch(factorTokens))
            {
                var op = tokens[current++];
                var right = Unary();
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        Expression Unary()
        {
            if (!IsMatch(unaryTokens))
                return Primary();

            var op = tokens[current++];
            var right = Unary();
            return new Unary(op, right);
        }

        Expression Primary()
        {
            var (type, range) = tokens[current];
            switch (type)
            {
                case TokenType.Number:
                    return new Literal(double.Parse(source[range]));
                case TokenType.String:
                    return new Literal(source[range]);
                case TokenType.True:
                    return new Literal(true);
                case TokenType.False:
                    return new Literal(false);
                case TokenType.Nil:
                    return new Literal(null);
                case TokenType.LeftParen:
                    return Grouping();
                default:
                    throw new ParseException(
                        "Expect values of Number, String, and Boolean, Nil, or grouped expressions.");
            }
        }

        Expression Grouping()
        {
            var expression = Expression();
            return IsSafeIndex() && tokens[current].Type == TokenType.RightParen
                ? new Grouping(expression)
                : throw new ParseException("Expect ')' after expression.");
        }

        return Expression();
    }
}
