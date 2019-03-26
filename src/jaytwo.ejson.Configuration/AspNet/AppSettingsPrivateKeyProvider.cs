#if NETFRAMEWORK
using System;
using System.Configuration;
using System.Linq;

namespace jaytwo.ejson.Configuration.AspNetCore
{
    public class AppSettingsPrivateKeyProvider : IPrivateKeyProvider
    {
        public AppSettingsPrivateKeyProvider()
        {
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
                var key = $"ejson:{publicKey}";
                if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                    privateKey = ConfigurationManager.AppSettings[key];
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
#endif