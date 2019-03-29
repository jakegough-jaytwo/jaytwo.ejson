﻿// https://github.com/search?q=tweetnacl+filename%3Acurve25519.cs&type=Code

namespace jaytwo.ejson.Crypto.TweetNaCl
{
    internal class curve25519
    {
        internal readonly int CRYPTO_BYTES = 32;
        internal readonly int CRYPTO_SCALARBYTES = 32;

        internal static byte[] basev = new byte[] { 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        internal static int[] minusp = new int[] { 19, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 128 };

        public static int crypto_scalarmult_base(byte[] q, byte[] n)
        {
            byte[] basevp = basev;
            return crypto_scalarmult(q, n, basevp);
        }

        internal static void add(int[] outv, int outvoffset, int[] a, int aoffset, int[] b, int boffset)
        {
            int u = 0;

            for (int j = 0; j < 31; ++j)
            {
                u += a[aoffset + j] + b[boffset + j];
                outv[outvoffset + j] = u & 255;
                u = (int)((uint)u >> 8);
            }

            u += a[aoffset + 31] + b[boffset + 31];
            outv[outvoffset + 31] = u;
        }

        internal static void sub(int[] outv, int outvoffset, int[] a, int aoffset, int[] b, int boffset)
        {
            int u = 218;

            for (int j = 0; j < 31; ++j)
            {
                u += a[aoffset + j] + 65280 - b[boffset + j];
                outv[outvoffset + j] = u & 255;
                u = (int)((uint)u >> 8);
            }

            u += a[aoffset + 31] - b[boffset + 31];
            outv[outvoffset + 31] = u;
        }

        internal static void squeeze(int[] a, int aoffset)
        {
            int u = 0;

            for (int j = 0; j < 31; ++j)
            {
                u += a[aoffset + j];
                a[aoffset + j] = u & 255;
                u = (int)((uint)u >> 8);
            }

            u += a[aoffset + 31];
            a[aoffset + 31] = u & 127;
            u = 19 * ((int)((uint)u >> 7));

            for (int j = 0; j < 31; ++j)
            {
                u += a[aoffset + j];
                a[aoffset + j] = u & 255;
                u = (int)((uint)u >> 8);
            }

            u += a[aoffset + 31];
            a[aoffset + 31] = u;
        }

        internal static void freeze(int[] a, int aoffset)
        {
            int[] aorig = new int[32];

            for (int j = 0; j < 32; ++j)
            {
                aorig[j] = a[aoffset + j];
            }

            int[] minuspp = minusp;

            add(a, 0, a, 0, minuspp, 0);

            int negative = (int)(-(((int)((uint)a[aoffset + 31] >> 7)) & 1));

            for (int j = 0; j < 32; ++j)
            {
                a[aoffset + j] ^= negative & (aorig[j] ^ a[aoffset + j]);
            }
        }

        internal static void mult(int[] outv, int outvoffset, int[] a, int aoffset, int[] b, int boffset)
        {
            int j;

            for (int i = 0; i < 32; ++i)
            {
                int u = 0;

                for (j = 0; j <= i; ++j)
                {
                    u += a[aoffset + j] * b[boffset + i - j];
                }

                for (j = i + 1; j < 32; ++j)
                {
                    u += 38 * a[aoffset + j] * b[boffset + i + 32 - j];
                }

                outv[outvoffset + i] = u;
            }

            squeeze(outv, outvoffset);
        }

        internal static void mult121665(int[] outv, int[] a)
        {
            int j;
            int u = 0;

            for (j = 0; j < 31; ++j)
            {
                u += 121665 * a[j];
                outv[j] = u & 255;
                u = (int)((uint)u >> 8);
            }

            u += 121665 * a[31];
            outv[31] = u & 127;
            u = 19 * ((int)((uint)u >> 7));

            for (j = 0; j < 31; ++j)
            {
                u += outv[j];
                outv[j] = u & 255;
                u = (int)((uint)u >> 8);
            }

            u += outv[j];
            outv[j] = u;
        }

        internal static void square(int[] outv, int outvoffset, int[] a, int aoffset)
        {
            int j;

            for (int i = 0; i < 32; ++i)
            {
                int u = 0;

                for (j = 0; j < i - j; ++j)
                {
                    u += a[aoffset + j] * a[aoffset + i - j];
                }

                for (j = i + 1; j < i + 32 - j; ++j)
                {
                    u += 38 * a[aoffset + j] * a[aoffset + i + 32 - j];
                }

                u *= 2;

                if ((i & 1) == 0)
                {
                    u += a[aoffset + i / 2] * a[aoffset + i / 2];
                    u += 38 * a[aoffset + i / 2 + 16] * a[aoffset + i / 2 + 16];
                }

                outv[outvoffset + i] = u;
            }

            squeeze(outv, outvoffset);
        }

        internal static void select(int[] p, int[] q, int[] r, int[] s, int b)
        {
            int bminus1 = b - 1;

            for (int j = 0; j < 64; ++j)
            {
                int t = bminus1 & (r[j] ^ s[j]);
                p[j] = s[j] ^ t;
                q[j] = r[j] ^ t;
            }
        }

        internal static void mainloop(int[] work, byte[] e)
        {
            int[] xzm1 = new int[64];
            int[] xzm = new int[64];
            int[] xzmb = new int[64];
            int[] xzm1b = new int[64];
            int[] xznb = new int[64];
            int[] xzn1b = new int[64];
            int[] a0 = new int[64];
            int[] a1 = new int[64];
            int[] b0 = new int[64];
            int[] b1 = new int[64];
            int[] c1 = new int[64];
            int[] r = new int[32];
            int[] s = new int[32];
            int[] t = new int[32];
            int[] u = new int[32];

            for (int j = 0; j < 32; ++j)
            {
                xzm1[j] = work[j];
            }

            xzm1[32] = 1;

            for (int j = 33; j < 64; ++j)
            {
                xzm1[j] = 0;
            }

            xzm[0] = 1;

            for (int j = 1; j < 64; ++j)
            {
                xzm[j] = 0;
            }

            int[] xzmbp = xzmb, a0p = a0, xzm1bp = xzm1b;
            int[] a1p = a1, b0p = b0, b1p = b1, c1p = c1;
            int[] xznbp = xznb, up = u, xzn1bp = xzn1b;
            int[] workp = work, sp = s, rp = r;

            for (int pos = 254; pos >= 0; --pos)
            {
                int b = ((int)((int)((uint)(e[pos / 8] & 0xFF) >> (pos & 7))));
                b &= 1;
                select(xzmb, xzm1b, xzm, xzm1, b);
                add(a0, 0, xzmb, 0, xzmbp, 32);
                sub(a0p, 32, xzmb, 0, xzmbp, 32);
                add(a1, 0, xzm1b, 0, xzm1bp, 32);
                sub(a1p, 32, xzm1b, 0, xzm1bp, 32);
                square(b0p, 0, a0p, 0);
                square(b0p, 32, a0p, 32);
                mult(b1p, 0, a1p, 0, a0p, 32);
                mult(b1p, 32, a1p, 32, a0p, 0);
                add(c1, 0, b1, 0, b1p, 32);
                sub(c1p, 32, b1, 0, b1p, 32);
                square(rp, 0, c1p, 32);
                sub(sp, 0, b0, 0, b0p, 32);
                mult121665(t, s);
                add(u, 0, t, 0, b0p, 0);
                mult(xznbp, 0, b0p, 0, b0p, 32);
                mult(xznbp, 32, sp, 0, up, 0);
                square(xzn1bp, 0, c1p, 0);
                mult(xzn1bp, 32, rp, 0, workp, 0);
                select(xzm, xzm1, xznb, xzn1b, b);
            }

            for (int j = 0; j < 64; ++j)
            {
                work[j] = xzm[j];
            }
        }

        internal static void recip(int[] outv, int outvoffset, int[] z, int zoffset)
        {
            int[] z2 = new int[32];
            int[] z9 = new int[32];
            int[] z11 = new int[32];
            int[] z2_5_0 = new int[32];
            int[] z2_10_0 = new int[32];
            int[] z2_20_0 = new int[32];
            int[] z2_50_0 = new int[32];
            int[] z2_100_0 = new int[32];
            int[] t0 = new int[32];
            int[] t1 = new int[32];

            /* 2 */
            int[] z2p = z2;
            square(z2p, 0, z, zoffset);

            /* 4 */
            square(t1, 0, z2, 0);

            /* 8 */
            square(t0, 0, t1, 0);

            /* 9 */
            int[] z9p = z9, t0p = t0;
            mult(z9p, 0, t0p, 0, z, zoffset);

            /* 11 */
            mult(z11, 0, z9, 0, z2, 0);

            /* 22 */
            square(t0, 0, z11, 0);

            /* 2^5 - 2^0 = 31 */
            mult(z2_5_0, 0, t0, 0, z9, 0);

            /* 2^6 - 2^1 */
            square(t0, 0, z2_5_0, 0);

            /* 2^7 - 2^2 */
            square(t1, 0, t0, 0);

            /* 2^8 - 2^3 */
            square(t0, 0, t1, 0);

            /* 2^9 - 2^4 */
            square(t1, 0, t0, 0);

            /* 2^10 - 2^5 */
            square(t0, 0, t1, 0);

            /* 2^10 - 2^0 */
            mult(z2_10_0, 0, t0, 0, z2_5_0, 0);

            /* 2^11 - 2^1 */
            square(t0, 0, z2_10_0, 0);

            /* 2^12 - 2^2 */
            square(t1, 0, t0, 0);

            /* 2^20 - 2^10 */
            for (int i = 2; i < 10; i += 2)
            {
                square(t0, 0, t1, 0);
                square(t1, 0, t0, 0);
            }

            /* 2^20 - 2^0 */
            mult(z2_20_0, 0, t1, 0, z2_10_0, 0);

            /* 2^21 - 2^1 */
            square(t0, 0, z2_20_0, 0);

            /* 2^22 - 2^2 */
            square(t1, 0, t0, 0);

            /* 2^40 - 2^20 */
            for (int i = 2; i < 20; i += 2)
            {
                square(t0, 0, t1, 0);
                square(t1, 0, t0, 0);
            }

            /* 2^40 - 2^0 */
            mult(t0, 0, t1, 0, z2_20_0, 0);

            /* 2^41 - 2^1 */
            square(t1, 0, t0, 0);

            /* 2^42 - 2^2 */
            square(t0, 0, t1, 0);

            /* 2^50 - 2^10 */
            for (int i = 2; i < 10; i += 2)
            {
                square(t1, 0, t0, 0);
                square(t0, 0, t1, 0);
            }

            /* 2^50 - 2^0 */
            mult(z2_50_0, 0, t0, 0, z2_10_0, 0);

            /* 2^51 - 2^1 */
            square(t0, 0, z2_50_0, 0);

            /* 2^52 - 2^2 */
            square(t1, 0, t0, 0);

            /* 2^100 - 2^50 */
            for (int i = 2; i < 50; i += 2)
            {
                square(t0, 0, t1, 0);
                square(t1, 0, t0, 0);
            }

            /* 2^100 - 2^0 */
            mult(z2_100_0, 0, t1, 0, z2_50_0, 0);

            /* 2^101 - 2^1 */
            square(t1, 0, z2_100_0, 0);

            /* 2^102 - 2^2 */
            square(t0, 0, t1, 0);

            /* 2^200 - 2^100 */
            for (int i = 2; i < 100; i += 2)
            {
                square(t1, 0, t0, 0);
                square(t0, 0, t1, 0);
            }

            /* 2^200 - 2^0 */
            mult(t1, 0, t0, 0, z2_100_0, 0);

            /* 2^201 - 2^1 */
            square(t0, 0, t1, 0);

            /* 2^202 - 2^2 */
            square(t1, 0, t0, 0);

            /* 2^250 - 2^50 */
            for (int i = 2; i < 50; i += 2)
            {
                square(t0, 0, t1, 0);
                square(t1, 0, t0, 0);
            }

            /* 2^250 - 2^0 */
            mult(t0, 0, t1, 0, z2_50_0, 0);

            /* 2^251 - 2^1 */
            square(t1, 0, t0, 0);

            /* 2^252 - 2^2 */
            square(t0, 0, t1, 0);

            /* 2^253 - 2^3 */
            square(t1, 0, t0, 0);

            /* 2^254 - 2^4 */
            square(t0, 0, t1, 0);

            /* 2^255 - 2^5 */
            square(t1, 0, t0, 0);

            /* 2^255 - 21 */
            int[] t1p = t1, z11p = z11;
            mult(outv, outvoffset, t1p, 0, z11p, 0);
        }

        public static int crypto_scalarmult(byte[] q, byte[] n, byte[] p)
        {
            int[] work = new int[96];
            byte[] e = new byte[32];

            for (int i = 0; i < 32; ++i)
            {
                e[i] = n[i];
            }

            e[0] &= unchecked((byte)248);
            e[31] &= 127;
            e[31] |= 64;

            for (int i = 0; i < 32; ++i)
            {
                work[i] = p[i] & 0xFF;
            }

            mainloop(work, e);

            recip(work, 32, work, 32);
            mult(work, 64, work, 0, work, 32);
            freeze(work, 64);

            for (int i = 0; i < 32; ++i)
            {
                q[i] = (byte)work[64 + i];
            }

            return 0;
        }
    }
}
