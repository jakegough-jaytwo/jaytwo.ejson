using System;
using System.Collections.Generic;
using System.Text;

namespace jaytwo.ejson
{
    public interface IPrivateKeyProvider
    {
        bool TryGetPrivateKey(string publicKey, out string privateKey);
    }

    public interface IWriteablePrivateKeyProvider : IPrivateKeyProvider
    {
        void SavePrivateKey(string publicKey, string privateKey);
    }
}
