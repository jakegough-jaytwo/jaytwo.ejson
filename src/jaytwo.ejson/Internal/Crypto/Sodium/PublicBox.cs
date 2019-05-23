// https://github.com/search?q=tweetnacl+filename%3APublicBox.cs&type=Code
#pragma warning disable SA1313 // Parameter names must begin with lower-case letter
#pragma warning disable SA1306 // Field names must begin with lower-case letter

using System;
using jaytwo.ejson.Crypto.TweetNaCl;

namespace jaytwo.ejson.Crypto.Sodium
{
    internal class PublicBox
    {
        private const int BEFORENMBYTES = 32;
        private const int BOXZEROBYTES = 16;
        private const int ZEROBYTES = 32;

        private readonly byte[] PrecomputedSharedKey = new byte[PublicBox.BEFORENMBYTES];

        public PublicBox(byte[] _Privatekey, byte[] _Publickey)
        {
            curve25519xsalsa20poly1305.crypto_box_beforenm(this.PrecomputedSharedKey, _Publickey, _Privatekey);
        }

        public byte[] Decrypt(byte[] _Cipher, byte[] _Nonce)
        {
            int cipherLength = _Cipher.Length;
            byte[] paddedbuffer = new byte[cipherLength + PublicBox.BOXZEROBYTES];
            Array.Copy(_Cipher, 0, paddedbuffer, PublicBox.BOXZEROBYTES, cipherLength);

            if (curve25519xsalsa20poly1305.crypto_box_afternm(paddedbuffer, paddedbuffer, paddedbuffer.Length, _Nonce, this.PrecomputedSharedKey) != 0)
            {
                throw new Exception("PublicBox Decryption failed");
            }

            byte[] output = new byte[paddedbuffer.Length - PublicBox.ZEROBYTES];
            Array.Copy(paddedbuffer, PublicBox.ZEROBYTES, output, 0, output.Length);
            return output;
        }

        public byte[] Encrypt(byte[] _Plain, byte[] _Nonce)
        {
            int plainLength = _Plain.Length;
            byte[] paddedbuffer = new byte[plainLength + PublicBox.ZEROBYTES];
            Array.Copy(_Plain, 0, paddedbuffer, PublicBox.ZEROBYTES, plainLength);

            if (curve25519xsalsa20poly1305.crypto_box_afternm(paddedbuffer, paddedbuffer, paddedbuffer.Length, _Nonce, this.PrecomputedSharedKey) != 0)
            {
                throw new Exception("PublicBox Encryption failed");
            }

            byte[] output = new byte[plainLength + PublicBox.BOXZEROBYTES];
            Array.Copy(paddedbuffer, PublicBox.ZEROBYTES - PublicBox.BOXZEROBYTES, output, 0, output.Length);
            return output;
        }
    }
}

#pragma warning restore SA1313 // Parameter names must begin with lower-case letter
#pragma warning restore SA1306 // Field names must begin with lower-case letter
