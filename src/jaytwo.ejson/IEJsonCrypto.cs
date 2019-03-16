using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace jaytwo.ejson
{
    public interface IEJsonCrypto
    {
        void Decrypt(Stream stream);
        void Encrypt(Stream stream);
        string GetDecryptedJson(Stream stream);        
        string GetEncryptedJson(Stream stream);        
        string GenerateKeyPair(bool write);
    }
}
