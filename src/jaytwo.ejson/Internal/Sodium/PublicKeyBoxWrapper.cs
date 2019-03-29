using jaytwo.ejson.Crypto;
using jaytwo.ejson.Crypto.Sodium;
using jaytwo.ejson.Crypto.TweetNaCl;

namespace jaytwo.ejson.Internal.Sodium
{
    internal class PublicKeyBoxWrapper : IPublicKeyBox
    {
        public const int KeySize = 32;
        public const int NonceSize = 24;

        public virtual byte[] Create(byte[] message, byte[] nonce, byte[] secretKey, byte[] publicKey)
        {
            return new PublicBox(secretKey, publicKey).Encrypt(message, nonce);
        }

        public virtual KeyPair GenerateKeyPair()
        {
            byte[] secretKey = new byte[KeySize];
            RandomData.GetBytes(secretKey);

            byte[] publickKey = new byte[KeySize];
            curve25519xsalsa20poly1305.crypto_box_getpublickey(publickKey, secretKey);

            return new KeyPair(publickKey, secretKey);
        }

        public virtual byte[] GenerateNonce()
        {
            return RandomData.Generate(NonceSize);
        }

        public virtual byte[] Open(byte[] cipherText, byte[] nonce, byte[] secretKey, byte[] publicKey)
        {
            return new PublicBox(secretKey, publicKey).Decrypt(cipherText, nonce);
        }
    }
}
