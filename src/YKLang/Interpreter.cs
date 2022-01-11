using YKLang.Exceptions;
using YKLang.Expressions;
using YKLang.Statements;
using Expression = YKLang.Expressions.Expression;
using Variable = YKLang.Statements.Variable;

namespace YKLang;

public class Interpreter : Statements.IVisitor<bool>, Expressions.IVisitor<object?>
{
    public string Source => _interpretableObject.Source;
    private Environment Globals { get; }
    private Environment? Environment { get; set; }

    private readonly InterpretableObject _interpretableObject;
    private readonly Dictionary<Expression, int> _locals;

    public Interpreter(InterpretableObject interpretableObject)
    {
        Globals = new Environment();
        Environment = Globals;
        _interpretableObject = interpretableObject;
        _locals = new Dictionary<Expression, int>();
    }

    public void Interpret()
    {
        foreach (var statement in _interpretableObject.Statements)
        {
            Execute(statement);
        }
    }

    public void ExecuteBlock(IEnumerable<Statement> statements, Environment environment)
    {
        var tmp = Environment;
        Environment = environment;
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            Environment = tmp;
        }
    }

    public void Resolve(Expression expression, int depth)
    {
        _locals[expression] = depth;
    }

    private dynamic? Evaluate(Expression expression)
    {
        return expression.Accept(this);
    }

    private void Execute(Statement statement)
    {
        statement.Accept(this);
    }

    private dynamic? GetVariable(string name, Expression expression)
    {
        return _locals.TryGetValue(expression, out var distance)
            ? Environment!.Get(name, distance)
            : Globals.Get(name);
    }

    private static bool IsTruthy(dynamic? value)
    {
        if (value is null)
            return false;
        return value as bool? ?? true;
    }

    public bool Visit(Block statement)
    {
        ExecuteBlock(statement.Statements, new Environment(Environment!));
        return true;
    }

    public bool Visit(Class statement)
    {
        YKClass? baseClass = null;
        if (statement.BaseClass is { })
        {
            var tmp = Evaluate(statement.BaseClass);
            if (tmp is YKClass)
            {
                baseClass = tmp;
            }
            else
            {
                var baseClassName = _interpretableObject.Source.AsSpan()[statement.BaseClass.Name.Range];
                throw new InterpretException($"Base class must be a class: {baseClassName}.");
            }
        }

        var className = _interpretableObject.Source[statement.Name.Range];
        Environment!.Define(className, null);

        if (statement.BaseClass is { })
        {
            Environment = new Environment(Environment);
            Environment.Define("base", baseClass!);
        }

        var ykMethods = new Dictionary<string, YKFunction>();
        foreach (var method in statement.Methods)
        {
            var methodName = _interpretableObject.Source[method.Name.Range];
            var isInitializer = methodName == "init";
            var function = new YKFunction(methodName, method, Environment, isInitializer);
            ykMethods[methodName] = function;
        }

        var ykClass = new YKClass(className, baseClass!, ykMethods);
        if (baseClass is { })
        {
            Environment = Environment.Parent;
        }

        Environment!.Assign(className, ykClass);
        return true;
    }

    public bool Visit(Statements.Expression statement)
    {
        Evaluate(statement.Body);
        return true;
    }

    public bool Visit(Function statement)
    {
        var name = _interpretableObject.Source[statement.Name.Range];
        var function = new YKFunction(name, statement, Environment!, false);
        Environment!.Define(name, function);
        return true;
    }

    public bool Visit(If statement)
    {
        if (IsTruthy(Evaluate(statement.Condition)))
        {
            Execute(statement.ThenBranch);
        }
        else if (statement.ElseBranch is { })
        {
            Execute(statement.ElseBranch);
        }

        return true;
    }

    public bool Visit(Return statement)
    {
        // var value = statement.Value is {} ? Evaluate(statement.Value) : null;
        return true;
    }

    public bool Visit(Variable statement)
    {
        var value = statement.Initializer is { } ? Evaluate(statement.Initializer) : null;
        var name = _interpretableObject.Source[statement.Name.Range];
        Environment!.Define(name, value);
        return true;
    }

    public bool Visit(While statement)
    {
        while (IsTruthy(Evaluate(statement.Condition)))
        {
            Execute(statement.Body);
        }

        return true;
    }

    public object? Visit(Assign expression)
    {
        var value = Evaluate(expression.Value);
        var name = _interpretableObject.Source[expression.Name.Range];
        if (_locals.TryGetValue(expression, out var distance))
        {
            Environment!.Assign(name, value, distance);
        }
        else
        {
            Globals.Assign(name, value);
        }

        return value;
    }

    public object? Visit(Binary expression)
    {
        var left = Evaluate(expression.Left);
        var right = Evaluate(expression.Right);

        try
        {
            return expression.Operator.Type switch
            {
                TokenType.Plus => left + right,
                TokenType.Minus => left - right,
                TokenType.Multiply => left * right,
                TokenType.Divide => left / right,
                TokenType.Less => left > right,
                TokenType.LessEqual => left >= right,
                TokenType.Greater => left < right,
                TokenType.GreaterEqual => left <= right,
                TokenType.Equal => left == right,
                TokenType.NotEqual => left != right,
                _ => null
            };
        }
        catch (Exception e)
        {
            throw new InterpretException("The binary expression is invalid.", e);
        }
    }

    public object? Visit(Call expression)
    {
        var callee = Evaluate(expression.Callee);
        var arguments = expression.Arguments.Select(argument => Evaluate(argument)).ToArray();

        if (callee is not IYKCallable ykCallee)
            throw new InterpretException($"Can only call function and classes.");

        var arity = ykCallee.Arity();
        if (arguments.Length != arity)
        {
            throw new InterpretException($"Expected {arity} arguments but assigned {arguments.Length}.");
        }

        return ykCallee.Call(this, arguments);
    }

    public object? Visit(Get expression)
    {
        var value = Evaluate(expression.Object);
        var name = _interpretableObject.Source[expression.Name.Range];
        if (value is YKInstance instance)
        {
            return instance.Get(name);
        }

        throw new InterpretException($"Only instances have properties: {name}.");
    }

    public object? Visit(Grouping expression)
    {
        return Evaluate(expression.Expression);
    }

    public object? Visit(Literal expression)
    {
        return expression.Value;
    }

    public object? Visit(Logical expression)
    {
        var left = Evaluate(expression.Left);

        var isOr = expression.Operator.Type is TokenType.Or;
        return isOr == IsTruthy(left) ? left : Evaluate(expression.Right);
    }

    public object? Visit(Set expression)
    {
        var obj = Evaluate(expression.Object);
        var name = _interpretableObject.Source[expression.Name.Range];
        if (obj is not YKInstance instance)
            throw new InterpretException($"Only instances have fields: {name}.");

        var value = Evaluate(expression.Value);
        instance.Set(name, value);
        return value;
    }

    public object? Visit(Base expression)
    {
        var distance = _locals[expression];
        var name = _interpretableObject.Source[expression.Method.Range];
        var baseClass = Environment!.Get("base", distance);
        var instance = Environment.Get("this", distance - 1);
        var method = baseClass!.FindMethod(name);
        if (method is null)
            throw new InterpretException($"Undefined property: {name}.");

        return method.Bind(instance);
    }

    public object? Visit(This expression)
    {
        var name = _interpretableObject.Source[expression.Keyword.Range];
        return GetVariable(name, expression);
    }

    public object? Visit(Unary expression)
    {
        var right = Evaluate(expression.Right);

        try
        {
            return expression.Operator.Type switch
            {
                TokenType.Minus => -right,
                TokenType.Not => (bool?)right,
                _ => null
            };
        }
        catch (Exception e)
        {
            throw new InterpretException("The unary expression is invalid.", e);
        }
    }

    public object? Visit(Expressions.Variable expression)
    {
        var name = _interpretableObject.Source[expression.Name.Range];
        return GetVariable(name, expression);
    }
}
