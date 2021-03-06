using System.Text;
using YKLang.Expressions;
using YKLang.Statements;
using Expression = YKLang.Expressions.Expression;
using Variable = YKLang.Statements.Variable;

namespace YKLang;

public class AstStringBuilder : Statements.IVisitor<string>, Expressions.IVisitor<string>
{
    private readonly InterpretableObject _interpretableObject;

    public AstStringBuilder(InterpretableObject interpretableObject)
    {
        _interpretableObject = interpretableObject;
    }

    public IEnumerable<string> Interpret()
    {
        return _interpretableObject.Statements.Select(BuildString);
    }

    private string BuildString(Expression expression)
    {
        return expression.Accept(this);
    }

    private string BuildString(Statement statement)
    {
        return statement.Accept(this);
    }

    public string Visit(Block statement)
    {
        var builder = new StringBuilder();
        builder.Append("(block ");
        foreach (var stmt in statement.Statements)
        {
            builder.Append(stmt.Accept(this));
        }

        builder.Append(')');
        return builder.ToString();
    }

    public string Visit(Class statement)
    {
        var builder = new StringBuilder();
        builder.Append($"(class {GetTokenString(statement.Name)}");
        if (statement.BaseClass is { })
        {
            builder.Append($" : {BuildString(statement.BaseClass)}");
        }

        foreach (var function in statement.Methods)
        {
            builder.Append($" {BuildString(function)}");
        }

        builder.Append(')');
        return builder.ToString();
    }

    public string Visit(Statements.Expression statement)
    {
        return Parenthesize(";", statement.Body);
    }

    public string Visit(Function statement)
    {
        var builder = new StringBuilder();
        builder.Append($"(function {GetTokenString(statement.Name)}(");
        if (statement.Parameters.Length > 0)
            builder.Append(string.Join(" ", statement.Parameters.Select(GetTokenString)));
        builder.Append(')');
        if (statement.Body.Length > 0)
        {
            builder.Append(' ');
            builder.Append(string.Join(" ", statement.Body.Select(x => x.Accept(this))));
        }

        builder.Append(')');
        return builder.ToString();
    }

    public string Visit(If statement)
    {
        return statement.ElseBranch is null
            ? Parenthesize("if", statement.Condition, statement.ThenBranch)
            : Parenthesize("if-else", statement.Condition, statement.ThenBranch, statement.ElseBranch);
    }

    public string Visit(Return statement)
    {
        return statement.Value is null ? "(return)" : Parenthesize("return", statement.Value);
    }

    public string Visit(Variable statement)
    {
        return statement.Initializer is null
            ? Parenthesize("var", statement.Name)
            : Parenthesize("var", statement.Name, "=", statement.Initializer);
    }

    public string Visit(While statement)
    {
        return Parenthesize("while", statement.Condition, statement.Body);
    }

    public string Visit(Assign expression)
    {
        return Parenthesize("=", GetTokenString(expression.Name), expression.Value);
    }

    public string Visit(Binary expression)
    {
        return Parenthesize(GetTokenString(expression.Operator), expression.Left, expression.Right);
    }

    public string Visit(Call expression)
    {
        return Parenthesize("call", expression.Callee, expression.Arguments);
    }

    public string Visit(Get expression)
    {
        return Parenthesize(".", expression.Object, GetTokenString(expression.Name));
    }

    public string Visit(Grouping expression)
    {
        return Parenthesize("group", expression.Expression);
    }

    public string Visit(Literal expression)
    {
        return expression.Value is null ? "nil" : expression.Value.ToString();
    }

    public string Visit(Logical expression)
    {
        return Parenthesize(GetTokenString(expression.Operator), expression.Left, expression.Right);
    }

    public string Visit(Set expression)
    {
        return Parenthesize("=", expression.Object, GetTokenString(expression.Name), expression.Value);
    }

    public string Visit(Base expression)
    {
        return Parenthesize("base", expression.Method);
    }

    public string Visit(This expression)
    {
        return "this";
    }

    public string Visit(Unary expression)
    {
        return Parenthesize(GetTokenString(expression.Operator), expression.Right);
    }

    public string Visit(Expressions.Variable expression)
    {
        return GetTokenString(expression.Name);
    }

    private string Parenthesize(string name, params dynamic[] parts)
    {
        return $"({name} {string.Join(" ", parts.Select(x => TransformToString(x)))})";
    }

    private string TransformToString(dynamic value)
    {
        return value switch
        {
            Expression expr => expr.Accept(this),
            Statement stmt => stmt.Accept(this),
            Token token => GetTokenString(token),
            IEnumerable<dynamic> values => string.Join(" ", values.Select(x => TransformToString(x))),
            _ => value.ToString()
        };
    }

    private string GetTokenString(Token token)
    {
        return _interpretableObject.Source[token.Range];
    }
}
