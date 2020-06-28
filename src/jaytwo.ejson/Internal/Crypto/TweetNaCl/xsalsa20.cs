// https://github.com/search?q=tweetnacl+filename%3Axsalsa20.cs&type=Code

#pragma warning disable SA1311 // Static readonly fields must begin with upper-case letter
#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
#pragma warning disable SA1310 // Field names must not contain underscore
#pragma warning disable SA1304 // Non-private readonly fields must begin with upper-case letter
#pragma warning disable SA1401 // Fields must be private
#pragma warning disable SA1003 // Symbols must be spaced correctly
#pragma warning disable SA1009 // Closing parenthesis must be spaced correctly
#pragma warning disable SA1413 // Use trailing comma in multi-line initializers

namespace jaytwo.ejson.Crypto.TweetNaCl
{
    internal class xsalsa20
    {
        public static readonly byte[] sigma =
        {
            (byte) 'e', (byte) 'x', (byte) 'p', (byte) 'a', (byte) 'n', (byte) 'd', (byte) ' ', (byte) '3', (byte) '2', (byte) '-', (byte) 'b', (byte) 'y', (byte) 't', (byte) 'e', (byte) ' ', (byte) 'k'
        };

        internal readonly int crypto_stream_xsalsa20_ref_KEYBYTES = 32;
        internal readonly int crypto_stream_xsalsa20_ref_NONCEBYTES = 24;

        public static int crypto_stream(byte[] c, int clen, byte[] n, byte[] k)
        {
            byte[] subkey = new byte[32];

            hsalsa20.crypto_core(subkey, n, k, xsalsa20.sigma);
            return salsa20.crypto_stream(c, clen, n, 16, subkey);
        }

        public static int crypto_stream_xor(byte[] c, byte[] m, long mlen, byte[] n, byte[] k)
        {
            byte[] subkey = new byte[32];

            hsalsa20.crypto_core(subkey, n, k, xsalsa20.sigma);
            return salsa20.crypto_stream_xor(c, m, (int)mlen, n, 16, subkey);
        }
    }
}

#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter
#pragma warning restore SA1311 // Static readonly fields must begin with upper-case letter
#pragma warning restore SA1401 // Fields must be private
#pragma warning restore SA1304 // Non-private readonly fields must begin with upper-case letter
#pragma warning restore SA1310 // Field names must not contain underscore
#pragma warning restore SA1009 // Closing parenthesis must be spaced correctly
#pragma warning restore SA1003 // Symbols must be spaced correctly
#pragma warning restore SA1413 // Use trailing comma in multi-line initializers
