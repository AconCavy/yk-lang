﻿namespace YKLang;

[Serializable]
public class ParseException : Exception
{
    public ParseException() : base() { }
    public ParseException(string message) : base(message) { }
    public ParseException(string message, Exception inner) : base(message, inner) { }

    protected ParseException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
    }
}