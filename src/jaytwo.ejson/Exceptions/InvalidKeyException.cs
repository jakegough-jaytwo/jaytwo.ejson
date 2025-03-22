using System;

namespace jaytwo.ejson.Exceptions;

public class InvalidKeyException : Exception
{
    public InvalidKeyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public InvalidKeyException(string message)
        : base(message)
    {
    }
}
