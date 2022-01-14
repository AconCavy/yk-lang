using YKLang.Statements;

namespace YKLang;

public class YKFunction : IYKCallable
{
    public string Name { get; }
    private readonly Function _declaration;
    private readonly Environment _environment;
    private readonly bool _isInitializer;

    public YKFunction(string name, Function declaration, Environment environment, bool isInitializer)
    {
        Name = name;
        _declaration = declaration;
        _environment = environment;
        _isInitializer = isInitializer;
    }

    public YKFunction Bind(YKInstance instance)
    {
        var env = new Environment(_environment);
        env.Define("this", instance);
        return new YKFunction(Name, _declaration, env, _isInitializer);
    }

    public int Arity()
    {
        return _declaration.Parameters.Length;
    }

    public dynamic? Call(Interpreter interpreter, dynamic?[] arguments)
    {
        var env = new Environment(_environment);
        foreach (var (parameter, argument) in _declaration.Parameters.Zip(arguments))
        {
            env.Define(interpreter.Source[parameter.Range], argument);
        }

        return null;
    }

    public override string ToString()
    {
        return Name;
    }
}
