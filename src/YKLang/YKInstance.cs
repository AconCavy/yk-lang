using YKLang.Exceptions;

namespace YKLang;

public class YKInstance
{
    private readonly YKClass _ykClass;
    private readonly Dictionary<string, dynamic> _fields;

    public YKInstance(YKClass ykClass)
    {
        _ykClass = ykClass;
        _fields = new Dictionary<string, dynamic>();
    }

    public dynamic Get(string name)
    {
        if (_fields.ContainsKey(name))
        {
            return _fields[name];
        }

        var method = _ykClass.FindMethod(name);
        return method?.Bind(this) ?? throw new InterpretException($"Undefined property: {name}.");
    }

    public void Set(string name, dynamic value)
    {
        _fields[name] = value;
    }
}
