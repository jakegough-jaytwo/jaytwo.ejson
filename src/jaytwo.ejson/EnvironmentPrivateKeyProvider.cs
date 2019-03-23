using System;
using System.Collections.Generic;
using System.Text;

namespace jaytwo.ejson
{
    public class EnvironmentPrivateKeyProvider : IPrivateKeyProvider
    {
        public const string EJsonKeyPrefixEnvironmentVairable = "EJSON_KEYPREFIX";
        public const string DefaultEJsonKeyEnvironmentVariablePrefix = "EJK_";

        private readonly string _eJsonKeyEnvironmentVariablePrefix;

        public EnvironmentPrivateKeyProvider()
            : this(null)
        {
        }

        public EnvironmentPrivateKeyProvider(string eJsonKeyEnvironmentVariablePrefix)
        {
            _eJsonKeyEnvironmentVariablePrefix = eJsonKeyEnvironmentVariablePrefix;

            if (string.IsNullOrWhiteSpace(_eJsonKeyEnvironmentVariablePrefix))
            {
                _eJsonKeyEnvironmentVariablePrefix = Environment.GetEnvironmentVariable(EJsonKeyPrefixEnvironmentVairable);
            }

            if (string.IsNullOrWhiteSpace(_eJsonKeyEnvironmentVariablePrefix))
            {
                _eJsonKeyEnvironmentVariablePrefix = DefaultEJsonKeyEnvironmentVariablePrefix;
            }
        }

        public bool TryGetPrivateKey(string publicKey, out string privateKey)
        {
            try
            {
                privateKey = Environment.GetEnvironmentVariable(_eJsonKeyEnvironmentVariablePrefix + publicKey);
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
