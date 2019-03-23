using System;
using System.Collections.Generic;
using System.Text;

namespace jaytwo.ejson
{
    public interface IPrivateKeyProvider
    {
        bool TryGetPrivateKey(string publicKey, out string privateKey);
        bool CanSavePrivateKey { get; }
        string SavePrivateKey(string publicKey, string privateKey);        
    }
}
