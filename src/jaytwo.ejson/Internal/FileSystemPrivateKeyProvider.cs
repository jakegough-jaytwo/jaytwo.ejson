using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace jaytwo.ejson.Internal
{
    internal class FileSystemPrivateKeyProvider : IWriteablePrivateKeyProvider, IPrivateKeyProvider
    {
        public const string EjsonKeyDirectoryEnvironmentVairable = "EJSON_KEYDIR";
        public const string DefaultEjsonKeyDirectory = ".";// "/opt/ejson/keys";

        private readonly string _ejsonKeyDirectory;
        private readonly IFileSystem _fileSystem;

        public FileSystemPrivateKeyProvider()
            : this(null)
        {
        }

        public FileSystemPrivateKeyProvider(string ejsonKeyDirectory)
            : this(ejsonKeyDirectory, new FileSystemWrapper())
        {
        }

        internal FileSystemPrivateKeyProvider(string ejsonKeyDirectory, IFileSystem fileSystem)
        {
            _ejsonKeyDirectory = ejsonKeyDirectory;
            _fileSystem = fileSystem;

            if (string.IsNullOrWhiteSpace(_ejsonKeyDirectory))
            {
                _ejsonKeyDirectory = Environment.GetEnvironmentVariable(EjsonKeyDirectoryEnvironmentVairable);
            }

            if (string.IsNullOrWhiteSpace(_ejsonKeyDirectory))
            {
                _ejsonKeyDirectory = DefaultEjsonKeyDirectory;
            }
        }

        public bool TryGetPrivateKey(string publicKey, out string privateKey)
        {
            try
            {
                var publicKeyPath = Path.Combine(_ejsonKeyDirectory, publicKey);

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

        public void SavePrivateKey(string publicKey, string privateKey)
        {
            var publicKeyPath = Path.Combine(_ejsonKeyDirectory, publicKey);
            _fileSystem.WriteAllText(publicKeyPath, privateKey);
        }
    }
}
