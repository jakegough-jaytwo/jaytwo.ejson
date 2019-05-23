using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using jaytwo.ejson.Internal;

namespace jaytwo.ejson
{
    public static class IEJsonCryptoExtensions
    {
        public static void EncryptFile(this IEJsonCrypto eJsonCrypto, string fileName)
        {
            EncryptFile(eJsonCrypto, fileName, null);
        }

        public static string GetDecryptedJsonFromFile(this IEJsonCrypto eJsonCrypto, string fileName, string keyDir)
        {
            return GetDecryptedJsonFromFile(eJsonCrypto, fileName, keyDir, null);
        }

        public static string GetDecryptedJsonFromFile(this IEJsonCrypto eJsonCrypto, string fileName, IPrivateKeyProvider keyProvider = null)
        {
            return GetDecryptedJsonFromFile(eJsonCrypto, fileName, keyProvider, null);
        }

        public static string GetDecryptedJson(this IEJsonCrypto eJsonCrypto, Stream stream, string keyDir)
        {
            return GetDecryptedJson(eJsonCrypto, stream, keyDir, null);
        }

        public static string GetDecryptedJson(this IEJsonCrypto eJsonCrypto, Stream stream, IPrivateKeyProvider keyProvider = null)
        {
            var json = new StreamReader(stream).ReadToEnd();
            return eJsonCrypto.GetDecryptedJson(json, keyProvider);
        }

        public static string SaveDecryptedJson(this IEJsonCrypto eJsonCrypto, string json, string outputFile, string keyDir)
        {
            return SaveDecryptedJson(eJsonCrypto, json, outputFile, keyDir, null);
        }

        public static string SaveDecryptedJsonFromFile(this IEJsonCrypto eJsonCrypto, string fileName, string outputFile, IPrivateKeyProvider keyProvider = null)
        {
            return SaveDecryptedJsonFromFile(eJsonCrypto, fileName, outputFile, keyProvider, null);
        }

        public static string SaveKeyPair(this IEJsonCrypto eJsonCrypto, string keyDir)
        {
            return SaveKeyPair(eJsonCrypto, keyDir, null);
        }

        public static string SaveDecryptedJsonFromFile(this IEJsonCrypto eJsonCrypto, string fileName, string outputFile, string keyDir)
        {
            return SaveDecryptedJsonFromFile(eJsonCrypto, fileName, outputFile, keyDir, null);
        }

        internal static void EncryptFile(this IEJsonCrypto eJsonCrypto, string fileName, IFileSystem fileSystem)
        {
            fileSystem = fileSystem ?? new FileSystemWrapper();
            var json = fileSystem.ReadAllText(fileName);
            var encrypted = eJsonCrypto.GetEncryptedJson(json);
            fileSystem.WriteAllText(fileName, encrypted);
        }

        internal static string GetDecryptedJsonFromFile(this IEJsonCrypto eJsonCrypto, string fileName, string keyDir, IFileSystem fileSystem)
        {
            return GetDecryptedJsonFromFile(eJsonCrypto, fileName, new FileSystemPrivateKeyProvider(keyDir, fileSystem), fileSystem);
        }

        internal static string GetDecryptedJsonFromFile(this IEJsonCrypto eJsonCrypto, string fileName, IPrivateKeyProvider keyProvider, IFileSystem fileSystem)
        {
            fileSystem = fileSystem ?? new FileSystemWrapper();
            var json = fileSystem.ReadAllText(fileName);
            return eJsonCrypto.GetDecryptedJson(json, keyProvider);
        }

        internal static string GetDecryptedJson(this IEJsonCrypto eJsonCrypto, Stream stream, string keyDir, IFileSystem fileSystem)
        {
            return GetDecryptedJson(eJsonCrypto, stream, new FileSystemPrivateKeyProvider(keyDir, fileSystem));
        }

        internal static string SaveDecryptedJsonFromFile(this IEJsonCrypto eJsonCrypto, string fileName, string outputFile, IPrivateKeyProvider keyProvider, IFileSystem fileSystem)
        {
            fileSystem = fileSystem ?? new FileSystemWrapper();
            var json = fileSystem.ReadAllText(fileName);
            return eJsonCrypto.SaveDecryptedJson(json, outputFile, keyProvider);
        }

        internal static string SaveKeyPair(this IEJsonCrypto eJsonCrypto, string keyDir, IFileSystem fileSystem)
        {
            fileSystem = fileSystem ?? new FileSystemWrapper();
            return eJsonCrypto.SaveKeyPair(new FileSystemPrivateKeyProvider(keyDir, fileSystem));
        }

        internal static string SaveDecryptedJson(this IEJsonCrypto eJsonCrypto, string json, string outputFile, string keyDir, IFileSystem fileSystem)
        {
            return eJsonCrypto.SaveDecryptedJson(json, outputFile, new FileSystemPrivateKeyProvider(keyDir, fileSystem));
        }

        internal static string SaveDecryptedJsonFromFile(this IEJsonCrypto eJsonCrypto, string fileName, string outputFile, string keyDir, IFileSystem fileSystem)
        {
            return SaveDecryptedJsonFromFile(eJsonCrypto, fileName, outputFile, new FileSystemPrivateKeyProvider(keyDir, fileSystem), fileSystem);
        }
    }
}
