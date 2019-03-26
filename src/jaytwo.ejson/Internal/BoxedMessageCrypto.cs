using System;
using System.Text;
using jaytwo.ejson.Internal.Sodium;

namespace jaytwo.ejson.Internal
{
    internal class BoxedMessageCrypto : IBoxedMessageCrypto
    {
        public const string SchemaVersion = "1";

        private readonly Encoding _encoding;
        private readonly IPublicKeyBox _publicKeyBox;

        public BoxedMessageCrypto()
            : this(Encoding.UTF8, new PublicKeyBoxWrapper())
        {
        }

        internal BoxedMessageCrypto(Encoding encoding, IPublicKeyBox publicKeyBox)
        {
            _publicKeyBox = publicKeyBox;
            _encoding = encoding;
        }

        public string Decrypt(BoxedMessage boxedMessage, byte[] privateKey)
        {
            if (boxedMessage.SchemaVersion == SchemaVersion)
            {
                var nonce = Convert.FromBase64String(boxedMessage.NonceBase64);
                var ephemeralPublicKey = Convert.FromBase64String(boxedMessage.PublicKeyBase64);
                var cipherText = Convert.FromBase64String(boxedMessage.EncryptedMessageBase64);
                var decryptedBytes = _publicKeyBox.Open(cipherText, nonce, privateKey, ephemeralPublicKey);
                var decryptedString = _encoding.GetString(decryptedBytes);
                return decryptedString;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public BoxedMessage Encrypt(string nessage, byte[] publicKey)
        {
            var messageBytes = _encoding.GetBytes(nessage);
            var nonce = _publicKeyBox.GenerateNonce();

            var ephemeralKeyPair = _publicKeyBox.GenerateKeyPair();
            var encryptedBytes = _publicKeyBox.Create(messageBytes, nonce, ephemeralKeyPair.SecretKey, publicKey);

            var boxedMessage = new BoxedMessage
            {
                EncryptedMessageBase64 = Convert.ToBase64String(encryptedBytes),
                PublicKeyBase64 = Convert.ToBase64String(ephemeralKeyPair.PublicKey),
                NonceBase64 = Convert.ToBase64String(nonce),
                SchemaVersion = SchemaVersion,
            };

            return boxedMessage;
        }
    }
}
