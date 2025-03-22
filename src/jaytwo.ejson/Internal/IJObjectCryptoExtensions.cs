using System;
using System.IO;
using jaytwo.ejson.Exceptions;
using Newtonsoft.Json.Linq;

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

        public static string EncryptJson(this IJObjectCrypto jObjectCrypto, string json, string publicKeyHex)
        {
            var publicKeyBytes = GetPublicKeyBytes(publicKeyHex);
            return jObjectCrypto.EncryptJson(json, publicKeyBytes);
        }

        public static string Encrypt(this IJObjectCrypto jObjectCrypto, Stream stream, string publicKeyHex)
        {
            var publicKeyBytes = GetPublicKeyBytes(publicKeyHex);
            return jObjectCrypto.Encrypt(stream, publicKeyBytes);
        }

        public static void Encrypt(this IJObjectCrypto jObjectCrypto, JObject jObject, string publicKeyHex)
        {
            var publicKeyBytes = GetPublicKeyBytes(publicKeyHex);
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

        public static string DecryptJson(this IJObjectCrypto jObjectCrypto, string json, string privateKeyHex)
        {
            var privateKeyBytes = GetPrivateKeyBytes(privateKeyHex);
            return jObjectCrypto.DecryptJson(json, privateKeyBytes);
        }

        public static string Decrypt(this IJObjectCrypto jObjectCrypto, Stream stream, string privateKey)
        {
            var privateKeyBytes = GetPrivateKeyBytes(privateKey);
            return jObjectCrypto.Decrypt(stream, privateKeyBytes);
        }

        public static void Decrypt(this IJObjectCrypto jObjectCrypto, JObject jObject, string privateKey)
        {
            var privateKeyBytes = GetPrivateKeyBytes(privateKey);
            jObjectCrypto.Decrypt(jObject, privateKeyBytes);
        }

        private static byte[] GetPublicKeyBytes(string hex)
            => HexToBinary(hex, "Invalid public key");

        private static byte[] GetPrivateKeyBytes(string hex)
            => HexToBinary(hex, "Invalid private key");

        private static byte[] HexToBinary(string hex, string onFailMessage)
        {
            try
            {
                return HexConverter.HexToBinary(hex);
            }
            catch (Exception ex)
            {
                throw new InvalidKeyException(onFailMessage, ex);
            }
        }
    }
}
