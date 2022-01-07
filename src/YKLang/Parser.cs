using YKLang.Exceptions;
using YKLang.Expressions;
using YKLang.Statements;
using Expression = YKLang.Expressions.Expression;
using Variable = YKLang.Expressions.Variable;

namespace YKLang;

public static class Parser
{
    public static InterpretableObject Parse(ParsableObject parsableObject)
    {
        var queue = new Queue<Token>(parsableObject.Tokens);
        var result = new List<Statement>();
        while (!IsMatch(TokenType.Eof))
        {
            var declaration = Declaration();
            if (declaration is { })
                result.Add(declaration);
        }

        return new InterpretableObject(parsableObject.Source, result);

        Token Advance() => queue.Count > 0 ? queue.Dequeue() : Token.Eof;
        Token Peek() => queue.Count > 0 ? queue.Peek() : Token.Eof;
        bool IsMatch(TokenType type) => type == Peek().Type;
        bool AnyMatch(params TokenType[] types) => types.Contains(Peek().Type);

        Token Expect(TokenType type, string message) =>
            IsMatch(type) ? Advance() : throw new ParseException(message);

        void Seek()
        {
            while (!IsMatch(TokenType.Eof))
            {
                switch (Peek().Type)
                {
                    case TokenType.Semicolon:
                        _ = Advance();
                        return;
                    case TokenType.Class:
                    case TokenType.Function:
                    case TokenType.Var:
                    case TokenType.For:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.Return:
                        return;
                }

                _ = Advance();
            }
        }

        Statement? Declaration()
        {
            try
            {
                return Peek().Type switch
                {
                    TokenType.Class => ClassDeclaration(),
                    TokenType.Function => FunctionDeclaration("function"),
                    TokenType.Var => VarDeclaration(),
                    _ => Statement()
                };
            }
            catch (ParseException)
            {
                Seek();
                return null;
            }
        }

        Class ClassDeclaration()
        {
            _ = Expect(TokenType.Class, "Expect 'class' keyword");
            var name = Expect(TokenType.Identifier, "Expect class name.");

            Variable? baseClass = null;
            if (IsMatch(TokenType.Colon))
            {
                _ = Advance();
                var baseName = Expect(TokenType.Identifier, "Expect class name.");
                baseClass = new Variable(baseName);
            }

            _ = Expect(TokenType.LeftBrace, "Expect '{' before class body.");

            var methods = new List<Function>();
            while (!AnyMatch(TokenType.RightBrace, TokenType.Eof))
            {
                methods.Add(FunctionDeclaration("Method"));
            }

            _ = Expect(TokenType.RightBrace, "Expect '}' after class body.");

            return new Class(name, baseClass, methods);
        }

        Function FunctionDeclaration(string kind)
        {
            _ = Expect(TokenType.Function, "Expect 'function' keyword");
            var name = Expect(TokenType.Identifier, $"Expect {kind} name.");
            _ = Expect(TokenType.LeftParen, $"Expect '(' after {kind} name.");

            var parameters = new List<Token>();
            while (IsMatch(TokenType.Identifier))
            {
                var parameter = Expect(TokenType.Identifier, "Expect parameter name.");
                parameters.Add(parameter);
                if (IsMatch(TokenType.Comma))
                    Advance();
            }

            _ = Expect(TokenType.RightParen, "Expect ')' after parameters.");
            var body = Block();

            return new Function(name, parameters, body);
        }

        Statements.Variable VarDeclaration()
        {
            _ = Expect(TokenType.Var, "Expect 'var' keyword");
            var name = Expect(TokenType.Identifier, "Expect variable name.");
            Expression? initializer = null;
            if (IsMatch(TokenType.Assign))
            {
                _ = Advance();
                initializer = Expression();
            }

            _ = Expect(TokenType.Semicolon, "Expect ';' after variable declaration.");

            return new Statements.Variable(name, initializer);
        }

        Statement Statement()
        {
            return Peek().Type switch
            {
                TokenType.For => ForStatement(),
                TokenType.If => IfStatement(),
                TokenType.Return => ReturnStatement(),
                TokenType.While => WhileStatement(),
                TokenType.LeftBrace => new Block(Block()),
                _ => ExpressionStatement()
            };
        }

        Statement ForStatement()
        {
            _ = Expect(TokenType.For, "Expect 'for' keyword");
            _ = Expect(TokenType.LeftParen, "Expect '(' after 'for' keyword.");

            Statement? initializer = null;
            switch (Peek().Type)
            {
                case TokenType.Semicolon:
                    _ = Advance();
                    break;
                case TokenType.Var:
                    initializer = VarDeclaration();
                    break;
                default:
                    initializer = ExpressionStatement();
                    break;
            }

            var condition = IsMatch(TokenType.Semicolon) ? null : Expression();

            _ = Expect(TokenType.Semicolon, "Expect ';' after loop condition.");

            var increment = IsMatch(TokenType.RightParen) ? null : Expression();

            _ = Expect(TokenType.RightParen, "Expect ')' after for clauses.");

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
            _ = Expect(TokenType.If, "Expect 'if' keyword");
            _ = Expect(TokenType.LeftParen, "Expect '(' after 'if' keyword.");
            var condition = Expression();
            _ = Expect(TokenType.RightParen, "Expect ')' after 'if' condition.");

            var thenBranch = Statement();
            Statement? elseBranch = null;
            if (IsMatch(TokenType.Else))
            {
                _ = Advance();
                elseBranch = IsMatch(TokenType.If) ? IfStatement() : Statement();
            }

            return new If(condition, thenBranch, elseBranch);
        }

        Statement ReturnStatement()
        {
            var keyword = Expect(TokenType.Return, "Expect 'return' keyword");
            var value = IsMatch(TokenType.Semicolon) ? null : Expression();
            _ = Expect(TokenType.Semicolon, "Expect ';' after return value.");

            return new Return(keyword, value);
        }

        Statement WhileStatement()
        {
            _ = Expect(TokenType.While, "Expect 'while' keyword");
            _ = Expect(TokenType.LeftParen, "Expect '(' after 'while' keyword.");
            var condition = Expression();
            _ = Expect(TokenType.RightParen, "Expect ')' after 'while' condition.");
            var body = Statement();

            return new While(condition, body);
        }

        Statement ExpressionStatement()
        {
            var expression = Expression();
            _ = Expect(TokenType.Semicolon, "Expect ';' after expression.");

            return new Statements.Expression(expression);
        }

        IEnumerable<Statement> Block()
        {
            _ = Expect(TokenType.LeftBrace, "Expect '{' before block");

            var statements = new List<Statement>();
            while (!AnyMatch(TokenType.RightBrace, TokenType.Eof))
            {
                var declaration = Declaration();
                if (declaration is { })
                    statements.Add(declaration);
            }

            _ = Expect(TokenType.RightBrace, "Expect '}' after block.");

            return statements;
        }

        Expression Expression()
        {
            return Assignment();
        }

        Expression Assignment()
        {
            var expression = Or();
            if (!IsMatch(TokenType.Assign))
                return expression;

            _ = Advance();
            var value = Assignment();
            return expression switch
            {
                Variable expr => new Assign(expr.Name, value),
                Get expr => new Set(expr.Object, expr.Name, value),
                _ => throw new ParseException("Invalid assignment target.")
            };
        }

        Expression Or()
        {
            var expression = And();
            while (IsMatch(TokenType.Or))
            {
                var op = Advance();
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
                var op = Advance();
                var right = Equality();
                expression = new Logical(expression, op, right);
            }

            return expression;
        }

        Expression Equality()
        {
            var expression = Comparison();
            while (AnyMatch(TokenType.Equal, TokenType.NotEqual))
            {
                var op = Advance();
                var right = Comparison();
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        Expression Comparison()
        {
            var expression = Term();
            while (AnyMatch(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
            {
                var op = Advance();
                var right = Term();
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        Expression Term()
        {
            var expression = Factor();
            while (AnyMatch(TokenType.Plus, TokenType.Minus))
            {
                var op = Advance();
                var right = Factor();
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        Expression Factor()
        {
            var expression = Unary();
            while (AnyMatch(TokenType.Multiply, TokenType.Divide))
            {
                var op = Advance();
                var right = Unary();
                expression = new Binary(expression, op, right);
            }

            return expression;
        }

        Expression Unary()
        {
            if (!AnyMatch(TokenType.Not, TokenType.Minus))
                return Call();

            var op = Advance();
            var right = Unary();

            return new Unary(op, right);
        }

        Expression Call()
        {
            var expression = Primary();
            while (true)
            {
                if (IsMatch(TokenType.LeftParen))
                {
                    _ = Advance();
                    var arguments = new List<Expression>();
                    if (!IsMatch(TokenType.RightParen))
                    {
                        do
                        {
                            arguments.Add(Expression());
                        } while (IsMatch(TokenType.Comma));
                    }

                    var paren = Expect(TokenType.RightParen, "Expect ')' after arguments.");
                    return new Call(expression, paren, arguments);
                }

                if (IsMatch(TokenType.Dot))
                {
                    _ = Advance();
                    var name = Expect(TokenType.Identifier, "Expect property name after '.'.");
                    return new Get(expression, name);
                }

                break;
            }

            return expression;
        }

        Expression Primary()
        {
            var token = Advance();
            return token.Type switch
            {
                TokenType.Number => new Literal(double.Parse(parsableObject.Source[token.Range])),
                TokenType.String => new Literal(parsableObject.Source[token.Range]),
                TokenType.True => new Literal(true),
                TokenType.False => new Literal(false),
                TokenType.Nil => new Literal(null),
                TokenType.Identifier => new Variable(token),
                TokenType.Base => Base(token),
                TokenType.This => new This(token),
                TokenType.LeftParen => Grouping(),
                _ => throw new ParseException("Expect expressions.")
            };

            Expression Base(Token keyword)
            {
                _ = Expect(TokenType.Dot, "Expect '.' after 'base' keyword.");
                var method = Expect(TokenType.Identifier, "Expect 'baseclass' method name.");

                return new Base(keyword, method);
            }

            Expression Grouping()
            {
                var expression = Expression();
                _ = Expect(TokenType.RightParen, "Expect ')' after expression.");

                return new Grouping(expression);
            }
        }
    }
}
