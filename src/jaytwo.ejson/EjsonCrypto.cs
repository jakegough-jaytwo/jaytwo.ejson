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

        public string GetDecryptedJson(Stream stream)
        {
            var jObject = GetDecryptJObject(stream);            
            return JObjectTools.GetJson(jObject);
        }

        public void Decrypt(Stream stream)
        {
            var jObject = GetDecryptJObject(stream);
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

        public string GenerateKeyPair(bool write)
        {
            using (var keyPair = _publicKeyBox.GenerateKeyPair())
            {
                var output = new StringBuilder();
                var publicKeyHex = Utilities.BinaryToHex(keyPair.PublicKey);
                var privateKeyHex = Utilities.BinaryToHex(keyPair.PrivateKey);

                if (write)
                {
                    _privateKeyProvider.SavePrivateKey(publicKeyHex, privateKeyHex);
                    output.AppendLine(publicKeyHex);
                }
                else
                {
                    output.AppendLine("Public Key:");
                    output.AppendLine(publicKeyHex);

                    output.AppendLine("Private Key:");
                    output.AppendLine(privateKeyHex);
                }

                return output.ToString().Trim();
            }
        }

        private JObject GetDecryptJObject(Stream stream)
        {
            var jObject = JObjectTools.GetJObject(stream);
            var publicKey = GetPublicKey(jObject);
            var privateKey = GetPrivateKey(publicKey);

            _jObjectCrypto.Decrypt(jObject, privateKey);

            return jObject;
        }

        private JObject GetEncryptedJObject(Stream stream)
        {
            var jObject = JObjectTools.GetJObject(stream);
            var publicKey = GetPublicKey(jObject);
            var privateKey = GetPrivateKey(publicKey);

            _jObjectCrypto.Encrypt(jObject, privateKey);

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

        private string GetPrivateKey(string publicKey)
        {
            if (_privateKeyProvider.TryGetPrivateKey(publicKey, out string privateKey))
            {
                return privateKey;
            }

            throw new InvalidOperationException($"Could not find private key for: {publicKey}");
        }
    }
}
