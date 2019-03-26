#if NETSTANDARD
using System;
using Microsoft.Extensions.Configuration;

namespace jaytwo.ejson.Configuration.AspNetCore
{
    public class ConfigurationPrivateKeyProvider : IPrivateKeyProvider
    {
        private readonly IConfiguration _configuration;

        public ConfigurationPrivateKeyProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool CanSavePrivateKey => false;

        public string SavePrivateKey(string publicKey, string privateKey)
        {
            throw new NotSupportedException();
        }

        public bool TryGetPrivateKey(string publicKey, out string privateKey)
        {
            try
            {
                privateKey = _configuration[publicKey];
                return !string.IsNullOrWhiteSpace(privateKey);
            }
            catch
            {
            }

            privateKey = null;
            return false;
        }
    }
}
#endif