using YKLang.Exceptions;

namespace YKLang;

public class Environment
{
    public Environment? Parent { get; }
    private readonly Dictionary<string, dynamic> _values;

    public Environment()
    {
        _values = new Dictionary<string, dynamic>();
    }

    public Environment(Environment parent) : this()
    {
        Parent = parent;
    }

    public void Assign(string name, dynamic value)
    {
        if (_values.ContainsKey(name))
        {
            _values[name] = value;
            return;
        }

        if (Parent is { })
        {
            Parent.Assign(name, value);
            return;
        }

        throw UndefinedException(name);
    }

    public void Assign(string name, dynamic value, int distance)
    {
        GetAncestor(distance).Assign(name, value);
    }

    public void Define(string name, dynamic value)
    {
        _values[name] = value;
    }

    public dynamic Get(string name)
    {
        try
        {
            return _values[name];
        }
        catch
        {
            throw UndefinedException(name);
        }
    }

    public dynamic Get(string name, int distance)
    {
        return GetAncestor(distance).Get(name);
    }

    public Environment GetAncestor(int distance)
    {
        var current = this;
        for (var i = 0; i < distance; i++)
        {
            current = current.Parent;
            if (current is null)
                throw new InterpretException($"The distance {distance} environment is invalid");
        }

        return current;
    }

    private static InterpretException UndefinedException(string name) => new($"Undefined variable: {name}.");
}
