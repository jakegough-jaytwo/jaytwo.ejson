using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace jaytwo.ejson.Internal
{
    internal interface IJObjectCrypto
    {
        void Encrypt(JObject jObject, byte[] publicKey);

        void Decrypt(JObject jObject, byte[] privateKey);
    }
}
