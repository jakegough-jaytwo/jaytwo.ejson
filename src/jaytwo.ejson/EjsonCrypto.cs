using System;
using System.Text;
using jaytwo.ejson.Exceptions;
using jaytwo.ejson.Internal;
using jaytwo.ejson.Internal.Sodium;
using Newtonsoft.Json.Linq;

namespace jaytwo.ejson
{
    public class EJsonCrypto : IEJsonCrypto
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJObjectCrypto _jObjectCrypto;
        private readonly IPublicKeyBox _publicKeyBox;

        public EJsonCrypto()
            : this(null, null, null)
        {
        }

        internal EJsonCrypto(
            IFileSystem fileSystem,
            IJObjectCrypto jObjectCrypto,
            IPublicKeyBox publicKeyBox)
        {
            _fileSystem = fileSystem ?? new FileSystemWrapper();
            _jObjectCrypto = jObjectCrypto ?? new JObjectCrypto();
            _publicKeyBox = publicKeyBox ?? new PublicKeyBoxWrapper();
        }

        public string GetDecryptedJson(string json, IPrivateKeyProvider keyProvider = null)
        {
            var jObject = GetDecryptJObject(json, keyProvider);
            return JObjectTools.GetJson(jObject);
        }

        public string SaveDecryptedJson(string json, string outputFile, IPrivateKeyProvider keyProvider = null)
        {
            var decryptedJson = GetDecryptedJson(json, keyProvider);
            _fileSystem.WriteAllTextWithFinalNewLine(outputFile, decryptedJson);
            return "Saved to: " + outputFile;
        }

        public string SaveEncryptedJson(string json, string outputFile)
        {
            var encryptedJson = GetEncryptedJson(json);
            _fileSystem.WriteAllTextWithFinalNewLine(outputFile, encryptedJson);
            return "Saved to: " + outputFile;
        }

        public string GetEncryptedJson(string json)
        {
            var jObject = GetEncryptedJObject(json);
            return JObjectTools.GetJson(jObject);
        }

        public string GenerateKeyPair()
        {
            var keyPair = _publicKeyBox.GenerateKeyPair();
            var publicKeyHex = HexConverter.BinaryToHex(keyPair.PublicKey);
            var privateKeyHex = HexConverter.BinaryToHex(keyPair.SecretKey);

            var output = new StringBuilder();

            output.AppendLine("Public Key:");
            output.AppendLine(publicKeyHex);

            output.AppendLine("Private Key:");
            output.AppendLine(privateKeyHex);

            return output.ToString().Trim();
        }

        public string SaveKeyPair(IPrivateKeyProvider keyProvider = null)
        {
            keyProvider = keyProvider ?? new DefaultPrivateKeyProvider();

            var keyPair = _publicKeyBox.GenerateKeyPair();
            var publicKeyHex = HexConverter.BinaryToHex(keyPair.PublicKey);
            var privateKeyHex = HexConverter.BinaryToHex(keyPair.SecretKey);

            keyProvider.SavePrivateKey(publicKeyHex, privateKeyHex);

            var output = new StringBuilder();

            output.AppendLine(publicKeyHex);

            return output.ToString().Trim();
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

        private JObject GetDecryptJObject(string json, IPrivateKeyProvider keyProvider)
        {
            var jObject = JObjectTools.GetJObject(json);
            var publicKey = GetPublicKey(jObject);

            keyProvider = keyProvider ?? new DefaultPrivateKeyProvider();
            if (keyProvider.TryGetPrivateKey(publicKey, out string privateKey))
            {
                _jObjectCrypto.Decrypt(jObject, privateKey);
                return jObject;
            }

            throw new InvalidOperationException($"Could not find private key for: {publicKey}");
        }

        private JObject GetEncryptedJObject(string json)
        {
            var jObject = JObjectTools.GetJObject(json);
            var publicKey = GetPublicKey(jObject);
            _jObjectCrypto.Encrypt(jObject, publicKey);

            return jObject;
        }
    }
}
