namespace YKLang.Exceptions;

[Serializable]
public class InterpretException : Exception
{
    public InterpretException() { }
    public InterpretException(string message) : base(message) { }
    public InterpretException(string message, Exception inner) : base(message, inner) { }

    protected InterpretException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
    }
}
