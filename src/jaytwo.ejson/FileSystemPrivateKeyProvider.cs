using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using jaytwo.ejson.Internal;

namespace jaytwo.ejson
{
    public class FileSystemPrivateKeyProvider : IPrivateKeyProvider
    {
        public const string EJsonKeyDirectoryEnvironmentVairable = "EJSON_KEYDIR";
        public const string DefaultUnixKeyDir = "/opt/ejson/keys";

        private readonly string _ejsonKeyDirectory;
        private readonly IFileSystem _fileSystem;

        public FileSystemPrivateKeyProvider(string ejsonKeyDirectory)
            : this(ejsonKeyDirectory, null)
        {
        }

        internal FileSystemPrivateKeyProvider(string ejsonKeyDirectory, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? new FileSystemWrapper();

            _ejsonKeyDirectory = ejsonKeyDirectory;

            if (string.IsNullOrWhiteSpace(_ejsonKeyDirectory))
            {
                _ejsonKeyDirectory = Environment.GetEnvironmentVariable(EJsonKeyDirectoryEnvironmentVairable);
            }
        }

        public bool CanSavePrivateKey => true;

        public string SavePrivateKey(string publicKey, string privateKey)
        {
            var keyDir = GetDefaultKeyDir();
            var publicKeyPath = _fileSystem.CombinePath(keyDir, publicKey);

            _fileSystem.CreateDirectory(keyDir);
            _fileSystem.WriteAllText(publicKeyPath, privateKey);

            return publicKeyPath;
        }

        public bool TryGetPrivateKey(string publicKey, out string privateKey)
        {
            foreach (var keyDir in GetPrioritizedKeyDirs())
            {
                if (TryGetPrivateKey(keyDir, publicKey, out privateKey))
                {
                    return true;
                }
            }

            privateKey = null;
            return false;
        }

        internal static string GetDefaultWindowsKeyDir()
        {
            return GetUserDefaultKeyDir("USERPROFILE");
        }

        internal static string GetDefaultOSXKeyDir()
        {
            return GetUserDefaultKeyDir("HOME");
        }

        internal string GetDefaultKeyDir()
        {
            return GetPrioritizedKeyDirs().FirstOrDefault();
        }

        internal IList<string> GetPrioritizedKeyDirs()
        {
            if (!string.IsNullOrWhiteSpace(_ejsonKeyDirectory))
            {
                return new[] { _ejsonKeyDirectory };
            }
            else
            {
                var keyDirs = new[]
                {
                    new { keyDir = GetDefaultWindowsKeyDir(), platform = "windows" },
                    new { keyDir = GetDefaultOSXKeyDir(), platform = "osx" },
                    new { keyDir = DefaultUnixKeyDir, platform = "unix" },
                };

#if NETSTANDARD
                return keyDirs
                    .Where(x => !string.IsNullOrWhiteSpace(x.keyDir))
                    .OrderByDescending(x => x.platform == "windows" && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    .ThenByDescending(x => x.platform == "osx" && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    .ThenByDescending(x => x.platform == "unix")
                    .Select(x => x.keyDir)
                    .ToList();
#endif

#if NETFRAMEWORK
                return keyDirs
                    .Where(x => !string.IsNullOrWhiteSpace(x.keyDir))
                    .OrderByDescending(x => x.platform == "windows")
                    .ThenByDescending(x => x.platform == "unix")
                    .Select(x => x.keyDir)
                    .ToList();
#endif
            }
        }

        private static string GetUserDefaultKeyDir(string envVarName)
        {
            var userProfile = Environment.GetEnvironmentVariable(envVarName);
            if (!string.IsNullOrWhiteSpace(userProfile))
            {
                return userProfile + "/.ejson/keys";
            }

            return null;
        }

        private bool TryGetPrivateKey(string keyDir, string publicKey, out string privateKey)
        {
            try
            {
                var publicKeyPath = Path.Combine(keyDir, publicKey);

                // 64 hex characters, 32-bits... 100 is just a sanity check in case it has a byte order mark something else nuts
                if (_fileSystem.FileExists(publicKeyPath) && _fileSystem.GetFileLength(publicKeyPath) < 100)
                {
                    privateKey = _fileSystem.ReadAllText(publicKeyPath);
                    return !string.IsNullOrWhiteSpace(privateKey);
                }
            }
            catch
            {
            }

            privateKey = null;
            return false;
        }
    }
}
