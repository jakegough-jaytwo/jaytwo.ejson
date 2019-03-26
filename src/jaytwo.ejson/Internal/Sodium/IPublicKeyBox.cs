using nacl;

namespace jaytwo.ejson.Internal.Sodium
{
    // mockable instance for functionality we get in the static PublicKeyBox methods from the Sodium library
    internal interface IPublicKeyBox
    {
        byte[] Create(byte[] message, byte[] nonce, byte[] secretKey, byte[] publicKey);
        KeyPair GenerateKeyPair();
        byte[] GenerateNonce();
        byte[] Open(byte[] cipherText, byte[] nonce, byte[] secretKey, byte[] publicKey);
    }
}
