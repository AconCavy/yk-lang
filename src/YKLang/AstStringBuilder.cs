using System.Text;
using YKLang.Expressions;
using YKLang.Statements;

namespace YKLang;

public class AstStringBuilder : IAstBuilder<string>
{
    public string Source { get; }

    public AstStringBuilder(string source)
    {
        Source = source;
    }

    public string ToString(Expressions.Expression expression)
    {
        return expression.Accept(this);
    }

    public string ToString(Statement statement)
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
        if (statement.Base is { })
        {
            builder.Append($" : {ToString(statement.Base)}");
        }

        foreach (var function in statement.Functions)
        {
            builder.Append($" {ToString(function)}");
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
        builder.Append($"(function {GetTokenString(statement.Name)} + \"(\"");
        if (statement.Params is { })
            builder.Append(string.Join(" ", statement.Params.Select(GetTokenString)));
        builder.Append(") ");
        if (statement.Body is { })
            builder.Append(string.Join(" ", statement.Body.Select(x => x.Accept(this))));
        builder.Append(')');
        return builder.ToString();
    }

    public string Visit(If statement)
    {
        return statement.Else is null
            ? Parenthesize("if", statement.Condition, statement.Then)
            : Parenthesize("if-else", statement.Condition, statement.Then, statement.Else);
    }

    public string Visit(Return statement)
    {
        return statement.Value is null ? "(return)" : Parenthesize("return", statement.Value);
    }

    public string Visit(Statements.Variable statement)
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
            Expressions.Expression expr => expr.Accept(this),
            Statement stmt => stmt.Accept(this),
            Token token => GetTokenString(token),
            IEnumerable<dynamic> values => TransformToString(values),
            _ => value.ToString()
        };
    }

    private string GetTokenString(Token token)
    {
        return Source[token.Range];
    }
}
