using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace jaytwo.ejson
{
    public interface IEJsonCrypto
    {
        void Decrypt(Stream stream, IPrivateKeyProvider keyProvider);
        void Encrypt(Stream stream);
        string GetDecryptedJson(Stream stream, IPrivateKeyProvider keyProvider);
        string SaveDecryptedJson(Stream stream, string outputFile, IPrivateKeyProvider keyProvider);
        string GetEncryptedJson(Stream stream);
        string GenerateKeyPair();
        string SaveKeyPair(string keyDir);
    }
}
