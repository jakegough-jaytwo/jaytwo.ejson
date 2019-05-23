﻿// https://github.com/search?q=tweetnacl+filename%3Asalsa20.cs&type=Code

#pragma warning disable SA1310 // Field names must not contain underscore
#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
#pragma warning disable SA1304 // Non-private readonly fields must begin with upper-case letter
#pragma warning disable SA1401 // Fields must be private

namespace jaytwo.ejson.Crypto.TweetNaCl
{
    internal class salsa20
    {
        internal const int ROUNDS = 20;
        internal readonly int crypto_core_salsa20_ref_CONSTBYTES = 16;
        internal readonly int crypto_core_salsa20_ref_INPUTBYTES = 16;
        internal readonly int crypto_core_salsa20_ref_KEYBYTES = 32;
        internal readonly int crypto_core_salsa20_ref_OUTPUTBYTES = 64;
        internal readonly int crypto_stream_salsa20_ref_KEYBYTES = 32;
        internal readonly int crypto_stream_salsa20_ref_NONCEBYTES = 8;

        public static int crypto_core(byte[] outv, byte[] inv, byte[] k, byte[] c)
        {
            int x0, x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15;
            int j0, j1, j2, j3, j4, j5, j6, j7, j8, j9, j10, j11, j12, j13, j14, j15;
            int i;

            j0 = x0 = salsa20.load_littleendian(c, 0);
            j1 = x1 = salsa20.load_littleendian(k, 0);
            j2 = x2 = salsa20.load_littleendian(k, 4);
            j3 = x3 = salsa20.load_littleendian(k, 8);
            j4 = x4 = salsa20.load_littleendian(k, 12);
            j5 = x5 = salsa20.load_littleendian(c, 4);
            j6 = x6 = salsa20.load_littleendian(inv, 0);
            j7 = x7 = salsa20.load_littleendian(inv, 4);
            j8 = x8 = salsa20.load_littleendian(inv, 8);
            j9 = x9 = salsa20.load_littleendian(inv, 12);
            j10 = x10 = salsa20.load_littleendian(c, 8);
            j11 = x11 = salsa20.load_littleendian(k, 16);
            j12 = x12 = salsa20.load_littleendian(k, 20);
            j13 = x13 = salsa20.load_littleendian(k, 24);
            j14 = x14 = salsa20.load_littleendian(k, 28);
            j15 = x15 = salsa20.load_littleendian(c, 12);

            for (i = salsa20.ROUNDS; i > 0; i -= 2)
            {
                x4 ^= (int)salsa20.rotate(x0 + x12, 7);
                x8 ^= (int)salsa20.rotate(x4 + x0, 9);
                x12 ^= (int)salsa20.rotate(x8 + x4, 13);
                x0 ^= (int)salsa20.rotate(x12 + x8, 18);
                x9 ^= (int)salsa20.rotate(x5 + x1, 7);
                x13 ^= (int)salsa20.rotate(x9 + x5, 9);
                x1 ^= (int)salsa20.rotate(x13 + x9, 13);
                x5 ^= (int)salsa20.rotate(x1 + x13, 18);
                x14 ^= (int)salsa20.rotate(x10 + x6, 7);
                x2 ^= (int)salsa20.rotate(x14 + x10, 9);
                x6 ^= (int)salsa20.rotate(x2 + x14, 13);
                x10 ^= (int)salsa20.rotate(x6 + x2, 18);
                x3 ^= (int)salsa20.rotate(x15 + x11, 7);
                x7 ^= (int)salsa20.rotate(x3 + x15, 9);
                x11 ^= (int)salsa20.rotate(x7 + x3, 13);
                x15 ^= (int)salsa20.rotate(x11 + x7, 18);
                x1 ^= (int)salsa20.rotate(x0 + x3, 7);
                x2 ^= (int)salsa20.rotate(x1 + x0, 9);
                x3 ^= (int)salsa20.rotate(x2 + x1, 13);
                x0 ^= (int)salsa20.rotate(x3 + x2, 18);
                x6 ^= (int)salsa20.rotate(x5 + x4, 7);
                x7 ^= (int)salsa20.rotate(x6 + x5, 9);
                x4 ^= (int)salsa20.rotate(x7 + x6, 13);
                x5 ^= (int)salsa20.rotate(x4 + x7, 18);
                x11 ^= (int)salsa20.rotate(x10 + x9, 7);
                x8 ^= (int)salsa20.rotate(x11 + x10, 9);
                x9 ^= (int)salsa20.rotate(x8 + x11, 13);
                x10 ^= (int)salsa20.rotate(x9 + x8, 18);
                x12 ^= (int)salsa20.rotate(x15 + x14, 7);
                x13 ^= (int)salsa20.rotate(x12 + x15, 9);
                x14 ^= (int)salsa20.rotate(x13 + x12, 13);
                x15 ^= (int)salsa20.rotate(x14 + x13, 18);
            }

            x0 += j0;
            x1 += j1;
            x2 += j2;
            x3 += j3;
            x4 += j4;
            x5 += j5;
            x6 += j6;
            x7 += j7;
            x8 += j8;
            x9 += j9;
            x10 += j10;
            x11 += j11;
            x12 += j12;
            x13 += j13;
            x14 += j14;
            x15 += j15;

            salsa20.store_littleendian(outv, 0, x0);
            salsa20.store_littleendian(outv, 4, x1);
            salsa20.store_littleendian(outv, 8, x2);
            salsa20.store_littleendian(outv, 12, x3);
            salsa20.store_littleendian(outv, 16, x4);
            salsa20.store_littleendian(outv, 20, x5);
            salsa20.store_littleendian(outv, 24, x6);
            salsa20.store_littleendian(outv, 28, x7);
            salsa20.store_littleendian(outv, 32, x8);
            salsa20.store_littleendian(outv, 36, x9);
            salsa20.store_littleendian(outv, 40, x10);
            salsa20.store_littleendian(outv, 44, x11);
            salsa20.store_littleendian(outv, 48, x12);
            salsa20.store_littleendian(outv, 52, x13);
            salsa20.store_littleendian(outv, 56, x14);
            salsa20.store_littleendian(outv, 60, x15);

            return 0;
        }

        public static int crypto_stream(byte[] c, int clen, byte[] n, int noffset, byte[] k)
        {
            byte[] inv = new byte[16];
            byte[] block = new byte[64];

            int coffset = 0;

            if (clen == 0)
            {
                return 0;
            }

            for (int i = 0; i < 8; ++i)
            {
                inv[i] = n[noffset + i];
            }

            for (int i = 8; i < 16; ++i)
            {
                inv[i] = 0;
            }

            while (clen >= 64)
            {
                salsa20.crypto_core(c, inv, k, xsalsa20.sigma);

                int u = 1;

                for (int i = 8; i < 16; ++i)
                {
                    u += inv[i] & 0xff;
                    inv[i] = (byte)u;
                    u = (int)((uint)u >> 8);
                }

                clen -= 64;
                coffset += 64;
            }

            if (clen != 0)
            {
                salsa20.crypto_core(block, inv, k, xsalsa20.sigma);

                for (int i = 0; i < clen; ++i)
                {
                    c[coffset + i] = block[i];
                }
            }

            return 0;
        }

        public static int crypto_stream_xor(byte[] c, byte[] m, int mlen, byte[] n, int noffset, byte[] k)
        {
            byte[] inv = new byte[16];
            byte[] block = new byte[64];

            int coffset = 0;
            int moffset = 0;

            if (mlen == 0)
            {
                return 0;
            }

            for (int i = 0; i < 8; ++i)
            {
                inv[i] = n[noffset + i];
            }

            for (int i = 8; i < 16; ++i)
            {
                inv[i] = 0;
            }

            while (mlen >= 64)
            {
                salsa20.crypto_core(block, inv, k, xsalsa20.sigma);

                for (int i = 0; i < 64; ++i)
                {
                    c[coffset + i] = (byte)(m[moffset + i] ^ block[i]);
                }

                int u = 1;

                for (int i = 8; i < 16; ++i)
                {
                    u += inv[i] & 0xff;
                    inv[i] = (byte)u;
                    u = (int)((uint)u >> 8);
                }

                mlen -= 64;
                coffset += 64;
                moffset += 64;
            }

            if (mlen != 0)
            {
                salsa20.crypto_core(block, inv, k, xsalsa20.sigma);

                for (int i = 0; i < mlen; ++i)
                {
                    c[coffset + i] = (byte)(m[moffset + i] ^ block[i]);
                }
            }

            return 0;
        }

        internal static int load_littleendian(byte[] x, int offset)
        {
            return (x[offset] & 0xff) | ((x[offset + 1] & 0xff) << 8) | ((x[offset + 2] & 0xff) << 16) | ((x[offset + 3] & 0xff) << 24);
        }

        internal static long rotate(int u, int c)
        {
            return (u << c) | (int)((uint)u >> (32 - c));
        }

        internal static void store_littleendian(byte[] x, int offset, int u)
        {
            x[offset] = (byte)u;
            u = (int)((uint)u >> 8);
            x[offset + 1] = (byte)u;
            u = (int)((uint)u >> 8);
            x[offset + 2] = (byte)u;
            u = (int)((uint)u >> 8);
            x[offset + 3] = (byte)u;
        }
    }
}

#pragma warning restore SA1401 // Fields must be private
#pragma warning restore SA1304 // Non-private readonly fields must begin with upper-case letter
#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter
#pragma warning restore SA1310 // Field names must not contain underscore
