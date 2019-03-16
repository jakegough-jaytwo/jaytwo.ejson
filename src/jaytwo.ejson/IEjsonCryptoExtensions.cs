using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace jaytwo.ejson
{
    public static class IEJsonCryptoExtensions
    {
        public static void Decrypt(this IEJsonCrypto eJsonCrypto, string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                eJsonCrypto.Decrypt(stream);
            }
        }

        public static void Encrypt(this IEJsonCrypto eJsonCrypto, string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                eJsonCrypto.Encrypt(stream);
            }
        }

        public static string GetDecryptedJson(this IEJsonCrypto eJsonCrypto, string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return eJsonCrypto.GetDecryptedJson(stream);
            }
        }

        public static string GetEncryptedJson(this IEJsonCrypto eJsonCrypto, string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return eJsonCrypto.GetEncryptedJson(stream);
            }
        }
    }
}
