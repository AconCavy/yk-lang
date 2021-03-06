namespace YKLang;

public class YKClass : IYKCallable
{
    public string Name { get; }
    public YKClass? Parent { get; }
    public IReadOnlyDictionary<string, YKFunction> Methods { get; }

    public YKClass(string name, YKClass? parent, IReadOnlyDictionary<string, YKFunction> methods)
    {
        Name = name;
        Parent = parent;
        Methods = methods;
    }

    public YKFunction? FindMethod(string name)
    {
        return Methods.ContainsKey(name) ? Methods[name] : Parent?.FindMethod(name);
    }

    public int Arity()
    {
        return FindMethod(Name)?.Arity() ?? 0;
    }

    public dynamic? Call(Interpreter interpreter, dynamic?[] arguments)
    {
        var instance = new YKInstance(this);
        var initializer = FindMethod(Name);
        initializer?.Bind(instance).Call(interpreter, arguments);

        return instance;
    }

    public override string ToString()
    {
        return Name;
    }
}
