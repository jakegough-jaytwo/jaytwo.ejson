// https://github.com/search?q=tweetnacl+filename%3Axsalsa20poly1305.cs&type=Code

#pragma warning disable SA1310 // Field names must not contain underscore
#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
#pragma warning disable SA1304 // Non-private readonly fields must begin with upper-case letter
#pragma warning disable SA1401 // Fields must be private

namespace jaytwo.ejson.Crypto.TweetNaCl
{
    internal class xsalsa20poly1305
    {
        internal readonly int crypto_secretbox_KEYBYTES = 32;
        internal readonly int crypto_secretbox_NONCEBYTES = 24;
        internal readonly int crypto_secretbox_ZEROBYTES = 32;
        internal readonly int crypto_secretbox_BOXZEROBYTES = 16;

        public static int crypto_secretbox(byte[] c, byte[] m, long mlen, byte[] n, byte[] k)
        {
            if (mlen < 32)
            {
                return -1;
            }

            xsalsa20.crypto_stream_xor(c, m, mlen, n, k);
            poly1305.crypto_onetimeauth(c, 16, c, 32, mlen - 32, c);

            for (int i = 0; i < 16; ++i)
            {
                c[i] = 0;
            }

            return 0;
        }

        public static int crypto_secretbox_open(byte[] m, byte[] c, long clen, byte[] n, byte[] k)
        {
            if (clen < 32)
            {
                return -1;
            }

            byte[] subkeyp = new byte[32];

            xsalsa20.crypto_stream(subkeyp, 32, n, k);

            if (poly1305.crypto_onetimeauth_verify(c, 16, c, 32, clen - 32, subkeyp) != 0)
            {
                return -1;
            }

            xsalsa20.crypto_stream_xor(m, c, clen, n, k);

            for (int i = 0; i < 32; ++i)
            {
                m[i] = 0;
            }

            return 0;
        }
    }
}

#pragma warning restore SA1401 // Fields must be private
#pragma warning restore SA1304 // Non-private readonly fields must begin with upper-case letter
#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter
#pragma warning restore SA1310 // Field names must not contain underscore
