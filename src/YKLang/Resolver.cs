using YKLang.Exceptions;
using YKLang.Expressions;
using YKLang.Statements;
using Expression = YKLang.Expressions.Expression;
using Variable = YKLang.Statements.Variable;

namespace YKLang;

public class Resolver : Statements.IVisitor<bool>, Expressions.IVisitor<bool>
{
    private FunctionType CurrentFunctionType { get; set; }
    private ClassType CurrentClassType { get; set; }

    private readonly Interpreter _interpreter;
    private readonly Stack<Dictionary<string, bool>> _scopes;

    public Resolver(Interpreter interpreter)
    {
        CurrentFunctionType = FunctionType.None;
        CurrentClassType = ClassType.None;
        _interpreter = interpreter;
        _scopes = new Stack<Dictionary<string, bool>>();
    }

    public void Resolve(IEnumerable<Statement> statements)
    {
        foreach (var statement in statements)
        {
            Resolve(statement);
        }
    }

    private void Resolve(Statement statement)
    {
        statement.Accept(this);
    }

    private void Resolve(Expression expression)
    {
        expression.Accept(this);
    }

    private void Resolve(Expression expression, string name)
    {
        foreach (var (scope, idx) in _scopes.Select((x, i) => (x, i)))
        {
            if (!scope.ContainsKey(name))
                continue;
            _interpreter.Resolve(expression, idx);
            return;
        }
    }

    private void Resolve(Function function, FunctionType type)
    {
        var enclosingFunction = CurrentFunctionType;
        CurrentFunctionType = type;
        BeginScope();
        foreach (var parameter in function.Parameters)
        {
            var parameterName = GetName(parameter).ToString();
            Declare(parameterName);
            Define(parameterName);
        }

        Resolve(function.Body);
        EndScope();

        CurrentFunctionType = enclosingFunction;
    }

    private void BeginScope()
    {
        _scopes.Push(new Dictionary<string, bool>());
    }

    private void EndScope()
    {
        _scopes.Pop();
    }

    private void Declare(string name)
    {
        if (!_scopes.TryPeek(out var scope))
            return;
        if (scope.ContainsKey(name))
            throw new ResolveException("Already a variable with this name in this scope.");

        scope[name] = false;
    }

    private void Define(string name)
    {
        if (!_scopes.TryPeek(out var scope))
            return;
        scope[name] = true;
    }

    private ReadOnlySpan<char> GetName(Token token)
    {
        return _interpreter.Source.AsSpan()[token.Range];
    }

    private enum FunctionType
    {
        None,
        Function,
        Initializer,
        Method
    }

    private enum ClassType
    {
        None,
        Class,
        BaseClass
    }

    public bool Visit(Block statement)
    {
        BeginScope();
        Resolve(statement.Statements);
        EndScope();
        return true;
    }

    public bool Visit(Class statement)
    {
        var enclosingClassType = CurrentClassType;
        CurrentClassType = ClassType.Class;

        var className = GetName(statement.Name).ToString();
        Declare(className);
        Define(className);

        if (statement.BaseClass is { })
        {
            var baseclassName = GetName(statement.BaseClass.Name);
            if (className.AsSpan().SequenceEqual(baseclassName))
                throw new ResolveException($"A class can't inherit from itself: {baseclassName}.");

            CurrentClassType = ClassType.BaseClass;
            Resolve(statement.BaseClass);

            BeginScope();
            _scopes.Peek()["base"] = true;
        }

        BeginScope();
        _scopes.Peek()["this"] = true;

        foreach (var method in statement.Methods)
        {
            var declaration = FunctionType.Method;
            var methodName = GetName(method.Name);
            if (methodName == "init")
                declaration = FunctionType.Initializer;

            Resolve(method, declaration);
        }

        EndScope();

        if (statement.BaseClass is { })
            EndScope();

        CurrentClassType = enclosingClassType;
        return true;
    }

    public bool Visit(Statements.Expression statement)
    {
        Resolve(statement.Body);
        return true;
    }

    public bool Visit(Function statement)
    {
        var name = GetName(statement.Name).ToString();
        Declare(name);
        Define(name);
        Resolve(statement, FunctionType.Function);
        return true;
    }

    public bool Visit(If statement)
    {
        Resolve(statement.Condition);
        Resolve(statement.ThenBranch);
        if (statement.ElseBranch is { })
            Resolve(statement.ElseBranch);
        return true;
    }

    public bool Visit(Return statement)
    {
        var keywordName = GetName(statement.Keyword);
        if (CurrentClassType is ClassType.None)
            throw new ResolveException($"Can't return from top-level code: {keywordName}.");

        if (statement.Value is null)
            return true;

        if (CurrentFunctionType is FunctionType.Initializer)
            throw new ResolveException($"Can't return from top-level code: {keywordName}.");
        Resolve(statement.Value);

        return true;
    }

    public bool Visit(Variable statement)
    {
        var name = GetName(statement.Name).ToString();
        if (statement.Initializer is { })
            Resolve(statement.Initializer);

        Define(name);
        return true;
    }

    public bool Visit(While statement)
    {
        Resolve(statement.Condition);
        Resolve(statement.Body);
        return true;
    }

    public bool Visit(Assign expression)
    {
        Resolve(expression.Value);
        var name = GetName(expression.Name).ToString();
        Resolve(expression, name);
        return true;
    }

    public bool Visit(Binary expression)
    {
        Resolve(expression.Left);
        Resolve(expression.Right);
        return true;
    }

    public bool Visit(Call expression)
    {
        Resolve(expression.Callee);

        foreach (var argument in expression.Arguments)
        {
            Resolve(argument);
        }

        return true;
    }

    public bool Visit(Get expression)
    {
        Resolve(expression.Object);
        return true;
    }

    public bool Visit(Grouping expression)
    {
        Resolve(expression.Expression);
        return true;
    }

    public bool Visit(Literal expression)
    {
        return true;
    }

    public bool Visit(Logical expression)
    {
        Resolve(expression.Left);
        Resolve(expression.Right);
        return true;
    }

    public bool Visit(Set expression)
    {
        Resolve(expression.Value);
        Resolve(expression.Object);
        return true;
    }

    public bool Visit(Base expression)
    {
        var keywordName = GetName(expression.Keyword).ToString();
        if (CurrentClassType is ClassType.None)
            throw new ResolveException($"Can't use 'base' outside of a class: {keywordName}.");
        if (CurrentClassType is not ClassType.BaseClass)
            throw new ResolveException($"Can't use 'base' in a class with no base class: {keywordName}.");

        Resolve(expression, keywordName);
        return true;
    }

    public bool Visit(This expression)
    {
        var keywordName = GetName(expression.Keyword).ToString();
        if (CurrentClassType is ClassType.None)
            throw new ResolveException($"Can't use 'this' outside of a class: {keywordName}.");

        Resolve(expression, keywordName);
        return true;
    }

    public bool Visit(Unary expression)
    {
        Resolve(expression.Right);
        return true;
    }

    public bool Visit(Expressions.Variable expression)
    {
        var name = GetName(expression.Name).ToString();
        if (_scopes.TryPeek(out var scope) && scope[name] == false)
            throw new ResolveException($"Can't read local variable in its own initializer: {name}.");

        Resolve(expression, name);
        return true;
    }
}
