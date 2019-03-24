using System;
using System.Collections.Generic;
using System.Linq;
using jaytwo.ejson.Internal;

namespace jaytwo.ejson
{
    public class DefaultPrivateKeyProvider : IPrivateKeyProvider
    {
        private readonly IList<IPrivateKeyProvider> _providers;

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
            _providers = new List<IPrivateKeyProvider>(providers);
        }

        public bool CanSavePrivateKey => _providers.Any(x => x.CanSavePrivateKey);

        public DefaultPrivateKeyProvider Add(IPrivateKeyProvider privateKeyProvider)
        {
            _providers.Add(privateKeyProvider);
            return this;
        }

        public string SavePrivateKey(string publicKey, string privateKey)
        {
            var writeableProvider = _providers.FirstOrDefault(x => x.CanSavePrivateKey);

            if (writeableProvider == null)
            {
                throw new InvalidOperationException("No writeable key providers!");
            }

            return writeableProvider.SavePrivateKey(publicKey, privateKey);
        }

        public bool TryGetPrivateKey(string publicKey, out string privateKey)
        {
            foreach (var provider in _providers)
            {
                if (provider != null && provider.TryGetPrivateKey(publicKey, out privateKey))
                {
                    return true;
                }
            }

            privateKey = null;
            return false;
        }
    }
}
