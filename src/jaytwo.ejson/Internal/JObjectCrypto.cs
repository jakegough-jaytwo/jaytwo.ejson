using System;
using Newtonsoft.Json.Linq;

namespace jaytwo.ejson.Internal
{
    internal class JObjectCrypto : IJObjectCrypto
    {
        public readonly IBoxedMessageCrypto _boxedMessageCrypto;

        public JObjectCrypto()
            : this(new BoxedMessageCrypto())
        {
        }

        internal JObjectCrypto(IBoxedMessageCrypto boxedMessageCrypto)
        {
            _boxedMessageCrypto = boxedMessageCrypto;
        }

        public void Encrypt(JObject jObject, byte[] publicKey)
        {
            EncryptTokenRecursive(jObject, publicKey);
        }

        private void EncryptTokenRecursive(JToken jToken, byte[] publicKey)
        {
            if (jToken.HasValues)
            {
                foreach (var child in jToken)
                {
                    EncryptTokenRecursive(child, publicKey);
                }
            }
            else if (IsEncryptable(jToken))
            {
                try
                {
                    var valueStirng = jToken.Value<string>();
                    if (!BoxedMessage.IsBoxedMessage(valueStirng))
                    {
                        var encryptedValue = _boxedMessageCrypto.Encrypt(valueStirng, publicKey).ToString();
                        ((JProperty)jToken.Parent).Value = encryptedValue;
                    };
                }
                catch
                {
                    // TODO: make error behavior configurable
                    throw;
                }
            }
        }

        public void Decrypt(JObject jObject, byte[] privateKey)
        {
            DecryptTokenRecursive(jObject, privateKey);
        }

        private void DecryptTokenRecursive(JToken jToken, byte[] privateKey)
        {
            if (jToken.HasValues)
            {
                foreach (var child in jToken)
                {
                    DecryptTokenRecursive(child, privateKey);
                }
            }
            else if (IsEncryptable(jToken))
            {
                try
                {
                    var valueStirng = jToken.Value<string>();
                    if (BoxedMessage.IsBoxedMessage(valueStirng) && BoxedMessage.TryCreate(valueStirng, out BoxedMessage boxedMessage))
                    {
                        var decryptedValue = _boxedMessageCrypto.Decrypt(boxedMessage, privateKey);
                        ((JProperty)jToken.Parent).Value = decryptedValue;
                    };
                }
                catch
                {
                    // TODO: make error behavior configurable
                    throw;
                }
            }
        }

        internal static bool IsEncryptable(JToken jToken)
        {
            /*
            from: https://shopify.github.io/ejson/ejson.5.html

            ENCRYPTABLE VALUES
            A value is considered encryptable if:

            It is a string literal (numbers, true, false, null all remain unencrypted);
            It is not an object key (ie. not immediately followed by a ":");
            Its corresponding object key did not begin with an underscore ("_").
            */

            if (jToken.Type != JTokenType.String)
            {
                return false;
            }

            var objectKey = (jToken.Parent as JProperty)?.Name;
            if (objectKey == null || objectKey.StartsWith("_"))
            {
                return false;
            }

            return true;
        }
    }
}
