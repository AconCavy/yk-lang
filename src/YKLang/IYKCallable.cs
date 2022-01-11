namespace YKLang;

public interface IYKCallable
{
    int Arity();
    dynamic? Call(Interpreter interpreter, IEnumerable<dynamic> arguments);
}
