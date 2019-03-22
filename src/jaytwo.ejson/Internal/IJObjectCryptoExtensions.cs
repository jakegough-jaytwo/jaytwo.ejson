using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Sodium;

namespace jaytwo.ejson.Internal
{
    internal static class IJObjectCryptoExtensions
    {
        public static string EncryptJson(this IJObjectCrypto jObjectCrypto, string json, byte[] publicKey)
        {
            var jObject = JObjectTools.GetJObject(json);
            jObjectCrypto.Encrypt(jObject, publicKey);
            return JObjectTools.GetJson(jObject);
        }

        public static string Encrypt(this IJObjectCrypto jObjectCrypto, Stream stream, byte[] publicKey)
        {
            var jObject = JObjectTools.GetJObject(stream);
            jObjectCrypto.Encrypt(jObject, publicKey);
            return JObjectTools.GetJson(jObject);
        }

        public static string EncryptJson(this IJObjectCrypto jObjectCrypto, string json, string publicKey)
        {
            var publicKeyBytes = Utilities.HexToBinary(publicKey);
            return jObjectCrypto.EncryptJson(json, publicKeyBytes);
        }
        public static string Encrypt(this IJObjectCrypto jObjectCrypto, Stream stream, string publicKey)
        {
            var publicKeyBytes = Utilities.HexToBinary(publicKey);
            return jObjectCrypto.Encrypt(stream, publicKeyBytes);
        }
        public static void Encrypt(this IJObjectCrypto jObjectCrypto, JObject jObject, string publicKey)
        {
            var publicKeyBytes = Utilities.HexToBinary(publicKey);
            jObjectCrypto.Encrypt(jObject, publicKeyBytes);
        }

        public static string DecryptJson(this IJObjectCrypto jObjectCrypto, string json, byte[] privateKey)
        {
            var jObject = JObjectTools.GetJObject(json);
            jObjectCrypto.Decrypt(jObject, privateKey);
            return JObjectTools.GetJson(jObject);
        }

        public static string Decrypt(this IJObjectCrypto jObjectCrypto, Stream stream, byte[] privateKey)
        {
            var jObject = JObjectTools.GetJObject(stream);
            jObjectCrypto.Decrypt(jObject, privateKey);
            return JObjectTools.GetJson(jObject);
        }

        public static string DecryptJson(this IJObjectCrypto jObjectCrypto, string json, string privateKey)
        {
            var privateKeyBytes = Utilities.HexToBinary(privateKey);
            return jObjectCrypto.DecryptJson(json, privateKeyBytes);
        }
        public static string Decrypt(this IJObjectCrypto jObjectCrypto, Stream stream, string privateKey)
        {
            var privateKeyBytes = Utilities.HexToBinary(privateKey);
            return jObjectCrypto.Decrypt(stream, privateKeyBytes);
        }
        public static void Decrypt(this IJObjectCrypto jObjectCrypto, JObject jObject, string privateKey)
        {
            var privateKeyBytes = Utilities.HexToBinary(privateKey);
            jObjectCrypto.Decrypt(jObject, privateKeyBytes);
        }
    }
}
