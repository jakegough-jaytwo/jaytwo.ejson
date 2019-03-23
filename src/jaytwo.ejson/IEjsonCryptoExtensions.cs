using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace jaytwo.ejson
{
    public static class IEJsonCryptoExtensions
    {
        public static void Decrypt(this IEJsonCrypto eJsonCrypto, string fileName, string keyDir)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                eJsonCrypto.Decrypt(stream, keyDir);
            }
        }

        public static void Encrypt(this IEJsonCrypto eJsonCrypto, string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                eJsonCrypto.Encrypt(stream);
            }
        }

        public static string GetDecryptedJson(this IEJsonCrypto eJsonCrypto, string fileName, string keyDir)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return eJsonCrypto.GetDecryptedJson(stream, keyDir);
            }
        }

        public static string SaveDecryptedJson(this IEJsonCrypto eJsonCrypto, string fileName, string outputFile, string keyDir)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return eJsonCrypto.SaveDecryptedJson(stream, outputFile, keyDir);
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
