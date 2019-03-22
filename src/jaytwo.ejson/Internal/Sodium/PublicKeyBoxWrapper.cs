using Sodium;
using System;
using System.Collections.Generic;
using System.Text;

namespace jaytwo.ejson.Internal.Sodium
{
    internal class PublicKeyBoxWrapper : IPublicKeyBox
    {
        public virtual byte[] Create(byte[] message, byte[] nonce, byte[] secretKey, byte[] publicKey) => PublicKeyBox.Create(message, nonce, secretKey, publicKey);
        public virtual KeyPair GenerateKeyPair() => PublicKeyBox.GenerateKeyPair();
        public virtual byte[] GenerateNonce() => PublicKeyBox.GenerateNonce();
        public virtual byte[] Open(byte[] cipherText, byte[] nonce, byte[] secretKey, byte[] publicKey) => PublicKeyBox.Open(cipherText, nonce, secretKey, publicKey);
    }
}
