using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace jaytwo.ejson
{
    public interface IEJsonCrypto
    {
        void Decrypt(Stream stream, string keyDir);
        void Encrypt(Stream stream);
        string GetDecryptedJson(Stream stream, string keyDir);
        string SaveDecryptedJson(Stream stream, string outputFile, string keyDir);
        string GetEncryptedJson(Stream stream);
        string GenerateKeyPair();
        string SaveKeyPair(string keyDir);
    }
}
