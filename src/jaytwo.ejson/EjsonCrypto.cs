using System;
using System.IO;
using System.Text;
using jaytwo.ejson.Exceptions;
using jaytwo.ejson.Internal;
using jaytwo.ejson.Internal.Sodium;
using Newtonsoft.Json.Linq;
using Sodium;

namespace jaytwo.ejson
{
    public class EJsonCrypto : IEJsonCrypto
    {
        private readonly IWriteablePrivateKeyProvider _privateKeyProvider;
        private readonly IJObjectCrypto _jObjectCrypto;
        private readonly IPublicKeyBox _publicKeyBox;

        public EJsonCrypto()
            : this(new DefaultPrivateKeyProvider())
        {
        }

        public EJsonCrypto(IWriteablePrivateKeyProvider privateKeyProvider)
            : this(
                  privateKeyProvider,
                  new JObjectCrypto(),
                  new PublicKeyBoxWrapper())
        {
        }

        internal EJsonCrypto(
            IWriteablePrivateKeyProvider privateKeyProvider,
            IJObjectCrypto jObjectCrypto,
            IPublicKeyBox publicKeyBox)
        {
            _privateKeyProvider = privateKeyProvider;
            _jObjectCrypto = jObjectCrypto;
            _publicKeyBox = publicKeyBox;
        }

        public string GetDecryptedJson(Stream stream, string keyDir)
        {
            var jObject = GetDecryptJObject(stream, keyDir);            
            return JObjectTools.GetJson(jObject);
        }

        public string SaveDecryptedJson(Stream stream, string outputFile, string keyDir)
        {
            var json = GetDecryptedJson(stream, keyDir);

            using (var file = File.CreateText(outputFile))
            {
                file.WriteLine(json);
            }

            return "Saved to: " + outputFile;
        }

        public void Decrypt(Stream stream, string keyDir)
        {
            var jObject = GetDecryptJObject(stream, keyDir);
            JObjectTools.Write(jObject, stream);
        }

        public string GetEncryptedJson(Stream stream)
        {
            var jObject = GetEncryptedJObject(stream);
            return JObjectTools.GetJson(jObject);
        }

        public void Encrypt(Stream stream)
        {
            var jObject = GetEncryptedJObject(stream);
            JObjectTools.Write(jObject, stream);
        }

        public string GenerateKeyPair()
        {
            using (var keyPair = _publicKeyBox.GenerateKeyPair())
            {
                var output = new StringBuilder();
                var publicKeyHex = Utilities.BinaryToHex(keyPair.PublicKey);
                var privateKeyHex = Utilities.BinaryToHex(keyPair.PrivateKey);

                output.AppendLine("Public Key:");
                output.AppendLine(publicKeyHex);

                output.AppendLine("Private Key:");
                output.AppendLine(privateKeyHex);

                return output.ToString().Trim();
            }
        }

        public string SaveKeyPair(string keyDir)
        {
            using (var keyPair = _publicKeyBox.GenerateKeyPair())
            {
                var output = new StringBuilder();
                var publicKeyHex = Utilities.BinaryToHex(keyPair.PublicKey);
                var privateKeyHex = Utilities.BinaryToHex(keyPair.PrivateKey);

                var filePath = Path.Combine(keyDir, publicKeyHex);
                using (var file = File.CreateText(filePath))
                {
                    file.Write(privateKeyHex);
                }

                output.AppendLine(publicKeyHex);

                return output.ToString().Trim();
            }
        }

        private JObject GetDecryptJObject(Stream stream, string keyDir)
        {
            var jObject = JObjectTools.GetJObject(stream);
            var publicKey = GetPublicKey(jObject);
            var privateKey = GetPrivateKey(publicKey, keyDir);

            _jObjectCrypto.Decrypt(jObject, privateKey);

            return jObject;
        }

        private JObject GetEncryptedJObject(Stream stream)
        {
            var jObject = JObjectTools.GetJObject(stream);
            var publicKey = GetPublicKey(jObject);            
            _jObjectCrypto.Encrypt(jObject, publicKey);

            return jObject;
        }

        private static string GetPublicKey(JObject jObject)
        {
            if (jObject.TryGetValue("_public_key", out JToken value))
            {
                if (value.Type == JTokenType.String)
                {
                    return value.Value<string>();
                }
            }

            throw new MissingPublicKeyException();
        }

        private string GetPrivateKey(string publicKey, string keyDir)
        {
            string result = null;

            var keyFile = Path.Combine(keyDir, publicKey);
            if (File.Exists(keyFile))
            {
                result = File.ReadAllText(keyFile)?.Trim();
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                throw new InvalidOperationException($"Could not find private key for: {publicKey}");
            }

            return result;
        }
    }
}
