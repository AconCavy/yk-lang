namespace YKLang.Exceptions;

public class ResolveException : Exception
{
    public ResolveException() { }
    public ResolveException(string message) : base(message) { }
    public ResolveException(string message, Exception inner) : base(message, inner) { }

    protected ResolveException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
    }
}
