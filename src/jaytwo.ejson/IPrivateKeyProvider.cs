using System;
using System.Collections.Generic;
using System.Text;

namespace jaytwo.ejson
{
    public interface IPrivateKeyProvider
    {
        bool CanSavePrivateKey { get; }

        bool TryGetPrivateKey(string publicKey, out string privateKey);

        string SavePrivateKey(string publicKey, string privateKey);
    }
}
