using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace jaytwo.ejson
{
    public interface IEJsonCrypto
    {
        string GetDecryptedJson(string json, IPrivateKeyProvider keyProvider = null);
        string SaveDecryptedJson(string json, string outputFile, IPrivateKeyProvider keyProvider = null);
        string GetEncryptedJson(string json);
        string SaveEncryptedJson(string json, string outputFile);
        string GenerateKeyPair();
        string SaveKeyPair(IPrivateKeyProvider keyProvider = null);
    }
}
