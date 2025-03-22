using System;

namespace jaytwo.ejson.Exceptions;

public class DecryptionFailedException : Exception
{
    public DecryptionFailedException(string publicKey, Exception innerException, string? additionalInformation = default)
        : base(GetMessage(publicKey, additionalInformation), innerException)
    {
        PublicKey = publicKey;
    }

    public DecryptionFailedException(string publicKey, string? additionalInformation = default)
        : base(GetMessage(publicKey, additionalInformation))
    {
        PublicKey = publicKey;
    }

    public string PublicKey { get; set; }

    private static string GetMessage(string publicKey, string? additionalInformation = default)
    {
        var additionalInformationMessage = string.IsNullOrEmpty(additionalInformation)
            ? string.Empty
            : $"{additionalInformation}; ";

        return $"Decryption Failed ({additionalInformationMessage}Public key: {publicKey})";
    }
}
