using System;

namespace jaytwo.ejson.Exceptions;

public class PrivateKeyNotFoundException : Exception
{
    public PrivateKeyNotFoundException(string publicKey)
        : base($"Private key not found for public key: {publicKey}")
    {
        PublicKey = publicKey;
    }

    public string PublicKey { get; set; }
}
