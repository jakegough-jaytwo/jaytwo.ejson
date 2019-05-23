// https://github.com/search?q=tweetnacl+filename%3Averify_16.cs&type=Code

#pragma warning disable SA1310 // Field names must not contain underscore
#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
#pragma warning disable SA1304 // Non-private readonly fields must begin with upper-case letter
#pragma warning disable SA1401 // Fields must be private

namespace jaytwo.ejson.Crypto.TweetNaCl
{
    internal class verify_16
    {
        internal readonly int crypto_verify_16_ref_BYTES = 16;

        public static int crypto_verify(byte[] x, int xoffset, byte[] y)
        {
            int differentbits = 0;

            for (int i = 0; i < 15; i++)
            {
                differentbits |= (x[xoffset + i] ^ y[i]) & 0xff;
            }

            return (1 & (int)((uint)(differentbits - 1) >> 8)) - 1;
        }
    }
}

#pragma warning restore SA1401 // Fields must be private
#pragma warning restore SA1304 // Non-private readonly fields must begin with upper-case letter
#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter
#pragma warning restore SA1310 // Field names must not contain underscore
