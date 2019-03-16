using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Sodium;

namespace jaytwo.ejson.Internal
{
    internal static class IJObjectCryptoExtensions
    {
        public static string EncryptJson(this IJObjectCrypto jObjectCrypto, string json, byte[] privateKey)
        {
            var jObject = JObjectTools.GetJObject(json);
            jObjectCrypto.Encrypt(jObject, privateKey);
            return JObjectTools.GetJson(jObject);
        }

        public static string Encrypt(this IJObjectCrypto jObjectCrypto, Stream stream, byte[] privateKey)
        {
            var jObject = JObjectTools.GetJObject(stream);
            jObjectCrypto.Encrypt(jObject, privateKey);
            return JObjectTools.GetJson(jObject);
        }

        public static string EncryptJson(this IJObjectCrypto jObjectCrypto, string json, string privateKey)
        {
            var privateKeyBytes = Utilities.HexToBinary(privateKey);
            return jObjectCrypto.EncryptJson(json, privateKeyBytes);
        }
        public static string Encrypt(this IJObjectCrypto jObjectCrypto, Stream stream, string privateKey)
        {
            var privateKeyBytes = Utilities.HexToBinary(privateKey);
            return jObjectCrypto.Encrypt(stream, privateKeyBytes);
        }
        public static void Encrypt(this IJObjectCrypto jObjectCrypto, JObject jObject, string privateKey)
        {
            var privateKeyBytes = Utilities.HexToBinary(privateKey);
            jObjectCrypto.Encrypt(jObject, privateKeyBytes);
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
