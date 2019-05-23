using System;
using System.Security.Cryptography;

namespace jaytwo.ejson.Crypto
{
    internal static class RandomData
    {
        public static byte[] Generate(int size)
        {
            var data = new byte[size];
            GetBytes(data);
            return data;
        }

        public static void GetBytes(byte[] data)
        {
            using (var generator = Create())
            {
                generator.GetBytes(data);
            }
        }

#if NETFRAMEWORK
        private static RNGCryptoServiceProvider Create() => new RNGCryptoServiceProvider();
#elif NETSTANDARD
        private static RandomNumberGenerator Create() => RandomNumberGenerator.Create();
#endif
    }
}
