using YKLang.Expressions;
using YKLang.Statements;
using Expression = YKLang.Expressions.Expression;

namespace YKLang;

public static class Parser
{
    public static Expression Parse(string source, IReadOnlyList<Token> tokens)
    {
        var current = 0;

        bool IsSafeIndex() => 0 <= current && current < tokens.Count;
        bool IsMatch(params TokenType[] types) => IsSafeIndex() && types.Contains(tokens[current].Type);

        Expression Expression()
        {
            return Assignment();
        }

        Statement? Declaration()
        {
            try
            {
                if (IsMatch(TokenType.Class))
                    return ClassDeclaration();
                if (IsMatch(TokenType.Function))
                    return Function("function");
                if (IsMatch(TokenType.Var))
                    return VarDeclaration();
                return Statement();
            }
            catch (ParseException)
            {
                Seek();
                return null;
            }
        }

        Statement ClassDeclaration()
        {
            if (!IsMatch(TokenType.Identifier))
                throw new ParseException("Expect class name.");
            var name = tokens[current++];

            Expressions.Variable? baseClass = null;
            if (IsMatch(TokenType.Colon))
            {
                if (!IsMatch(TokenType.Identifier))
                    throw new ParseException("Expect class name.");
                baseClass = new Expressions.Variable(tokens[current++]);
            }

            if (!IsMatch(TokenType.LeftBrace))
                throw new ParseException("Expect '{' before class body.");

            var methods = new List<Function>();
            while (!IsMatch(TokenType.RightBrace))
            {
                methods.Add(Function("Method"));
            }

            if (!IsMatch(TokenType.RightBrace))
                throw new ParseException("Expect '{' before class body.");

            return new Class(name, baseClass, methods);
        }

        Statement Statement()
        {
            if (IsMatch(TokenType.For))
                return ForStatement();
            if (IsMatch(TokenType.If))
                return IfStatement();
            if (IsMatch(TokenType.Return))
                return ReturnStatement();
            if (IsMatch(TokenType.While))
                return WhileStatement();
            if (IsMatch(TokenType.LeftBrace))
                return new Block(Block());

            return ExpressionStatement();
        }

        Statement ForStatement()
        {
            if (!IsMatch(TokenType.LeftParen))
                throw new ParseException("Expect '(' after 'for' keyword.");

            Statement? initializer = tokens[current].Type switch
            {
                TokenType.Semicolon => null,
                TokenType.Var => null,
                _ => null
            };

            Expression? condition = IsMatch(TokenType.Semicolon)
                ? null
                : throw new ParseException("Expect ';' after loop condition.");

            Expression? increment = IsMatch(TokenType.RightParen)
                ? null
                : throw new ParseException("Expect ')' after for clauses.");

            var body = Statement();

            if (increment is { })
                body = new Block(new[] { body, new Statements.Expression(increment) });

            condition ??= new Literal(true);
            body = new While(condition, body);

            if (initializer is { })
                body = new Block(new[] { initializer, body });

            return body;
        }

        Statement IfStatement()
        {
            if (!IsMatch(TokenType.LeftParen))
                throw new ParseException("Expect '(' after 'if' keyword.");

            var condition = Expression();

            if (!IsMatch(TokenType.RightParen))
                throw new ParseException("Expect ')' after 'if' condition.");

            var thenBranch = Statement();
            var elseBranch = IsMatch(TokenType.Else) ? Statement() : null;

            return new If(condition, thenBranch, elseBranch);
        }

        Statement ReturnStatement()
        {
            var keyword = tokens[current++];
            var value = IsMatch(TokenType.Semicolon) ? null : Expression();
            if (!IsMatch(TokenType.Semicolon))
                throw new ParseException("Expect ';' after return value.");

            return new Return(keyword, value);
        }

        Statement VarDeclaration()
        {
            if (!IsMatch(TokenType.Identifier))
                throw new ParseException($"Expect variable name.");

            var name = tokens[current++];
            var initializer = IsMatch(TokenType.Assign) ? Expression() : null;

            if (!IsMatch(TokenType.Semicolon))
                throw new ParseException("Expect ';' after variable declaration.");

            return new Statements.Variable(name, initializer);
        }

        Statement WhileStatement()
        {
            if (!IsMatch(TokenType.LeftParen))
                throw new ParseException("Expect '(' after 'while' keyword.");

            var condition = Expression();

            if (!IsMatch(TokenType.RightParen))
                throw new ParseException("Expect ')' after 'while' condition.");

            var body = Statement();

            return new While(condition, body);
        }

        Statement ExpressionStatement()
        {
            var expression = Expression();
            if (!IsMatch(TokenType.Semicolon))
                throw new ParseException("Expect ';' after expression.");

            return new Statements.Expression(expression);
        }

        Function Function(string kind)
        {
            if (!IsMatch(TokenType.Identifier))
                throw new ParseException($"Expect {kind} name.");
            var name = tokens[current++];

            if (!IsMatch(TokenType.LeftParen))
                throw new ParseException($"Expect '(' after {kind} name.");

            var arguments = new List<Token>();
            if (!IsMatch(TokenType.RightParen))
            {
                do
                {
                    if (!IsMatch(TokenType.Identifier))
                        throw new ParseException($"Expect argument name.");
                    arguments.Add(tokens[current++]);
                } while (IsMatch(TokenType.Comma));
            }

            if (!IsMatch(TokenType.RightParen))
                throw new ParseException($"Expect ')' after arguments.");

            var body = Block();
            return new Function(name, arguments, body);
        }

        IEnumerable<Statement> Block()
        {
            var statements = new List<Statement>();
            while (!IsMatch(TokenType.RightBrace))
            {
                var declaration = Declaration();
                if (declaration is { })
                    statements.Add(declaration);
            }

            if (!IsMatch(TokenType.RightBrace))
                throw new ParseException("Expect '}' after block.");

            return statements;
        }

        Expression Assignment()
        {
            var expression = Or();
            if (!IsMatch(TokenType.Assign))
                return expression;

            var value = Assignment();
            return expression switch
            {
                Expressions.Variable ev => new Assign(ev.Name, value),
                Get eg => new Set(eg.Object, eg.Name, value),
                _ => throw new ParseException("Invalid assignment target.")
            };
        }

        Expression Or()
        {
            var expression = And();
            while (IsMatch(TokenType.Or))
            {
                var op = tokens[current++];
                var right = And();
                expression = new Logical(expression, op, right);
            }

            return expression;
        }

        Expression And()
        {
            var expression = Equality();
            while (IsMatch(TokenType.And))
            {
                var op = tokens[current++];
                var right = Equality();
                expression = new Logical(expression, op, right);
            }

            return expression;
        }

        Expression Equality()
        {
            var expression = Comparison();
            while (IsMatch(TokenType.Equal, TokenType.NotEqual))
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
            while (IsMatch(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
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
            while (IsMatch(TokenType.Plus, TokenType.Minus))
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
            while (IsMatch(TokenType.Multiply, TokenType.Divide))
            {
                var op = tokens[current++];
                var right = Unary();
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        Expression Unary()
        {
            if (!IsMatch(TokenType.Not, TokenType.Minus))
                return Primary();

            var op = tokens[current++];
            var right = Unary();
            return new Unary(op, right);
        }

        Expression Primary()
        {
            var (type, range) = tokens[current++];
            return type switch
            {
                TokenType.Number => new Literal(double.Parse(source[range])),
                TokenType.String => new Literal(source[range]),
                TokenType.True => new Literal(true),
                TokenType.False => new Literal(false),
                TokenType.Nil => new Literal(null),
                TokenType.LeftParen => Grouping(),
                _ => throw new ParseException(
                    "Expect values of Number, String, and Boolean, Nil, or grouped expressions.")
            };
        }

        Expression Grouping()
        {
            var expression = Expression();
            return IsMatch(TokenType.RightParen)
                ? new Grouping(expression)
                : throw new ParseException("Expect ')' after expression.");
        }

        void Seek()
        {
            current++;
            while (IsSafeIndex())
            {
                var (type, _) = tokens[current];
                if (type is TokenType.Semicolon)
                    return;
                switch (type)
                {
                    case TokenType.Class:
                    case TokenType.Function:
                    case TokenType.Var:
                    case TokenType.For:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.Return:
                        return;
                }

                current++;
            }
        }

        return Expression();
    }
}
