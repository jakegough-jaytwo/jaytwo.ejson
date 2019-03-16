using System;
using System.Linq;
using jaytwo.ejson.Internal;

namespace jaytwo.ejson
{
    public class DefaultPrivateKeyProvider : IWriteablePrivateKeyProvider, IPrivateKeyProvider
    {
        private readonly IPrivateKeyProvider[] _providers;

        public DefaultPrivateKeyProvider()
            : this((string)null, (string)null)
        {
        }

        public DefaultPrivateKeyProvider(string eJsonKeyEnvironmentVariablePrefix = null, string ejsonKeyDirectory = null)
            : this(
                  new EnvironmentPrivateKeyProvider(eJsonKeyEnvironmentVariablePrefix),
                  new FileSystemPrivateKeyProvider(ejsonKeyDirectory))
        {
        }

        internal DefaultPrivateKeyProvider(params IPrivateKeyProvider[] providers)
        {
            _providers = providers;
        }

        public void SavePrivateKey(string publicKey, string privateKey)
        {
            var writeableProvider = _providers
                .Where(x => x as IWriteablePrivateKeyProvider != null)
                .Cast<IWriteablePrivateKeyProvider>()
                .FirstOrDefault();

            if (writeableProvider == null)
            {
                throw new InvalidOperationException("No writeable key providers!");
            }

            writeableProvider.SavePrivateKey(publicKey, privateKey);
        }

        public bool TryGetPrivateKey(string publicKey, out string privateKey)
        {
            foreach (var provider in _providers)
            {
                if (provider.TryGetPrivateKey(publicKey, out privateKey))
                {
                    return true;
                }
            }

            privateKey = null;
            return false;
        }
    }
}
