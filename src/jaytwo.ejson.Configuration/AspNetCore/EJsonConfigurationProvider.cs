#if NETSTANDARD
using System;
using System.IO;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;

namespace jaytwo.ejson.Configuration.AspNetCore
{
    public class EJsonConfigurationProvider : JsonConfigurationProvider
    {
        private readonly IEJsonCrypto _eJsonCrypto;

        public EJsonConfigurationProvider(EJsonConfigurationSource source)
            : this(source, null)
        {
        }

        internal EJsonConfigurationProvider(EJsonConfigurationSource source, IEJsonCrypto eJsonCrypto)
            : base(source)
        {
            _eJsonCrypto = eJsonCrypto ?? new EJsonCrypto();
        }

        public override void Load(Stream stream)
        {
            var source = Source as EJsonConfigurationSource;
            var logger = source?.LoggerFactory?.CreateLogger(this.GetType());
            var path = source?.Path;

            try
            {
                var privateKeyProvider = GetKeyProvider(source);
                var decryptedJson = _eJsonCrypto.GetDecryptedJson(stream, privateKeyProvider);

                using (var memoryStream = new MemoryStream())
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    streamWriter.Write(decryptedJson);
                    streamWriter.Flush();
                    memoryStream.Position = 0;

                    base.Load(memoryStream);
                }

                logger?.LogInformation(default(EventId), "EJSON loaded from {path}", path);
            }
            catch (Exception exception)
            {
                logger?.LogError(default(EventId), exception, "Could not load EJSON from {path}", path);
            }
        }

        private static IPrivateKeyProvider GetKeyProvider(EJsonConfigurationSource source)
        {
            var result = new DefaultPrivateKeyProvider();

            var configSection = (source)?.ConfigSection;
            if (configSection != null)
            {
                result.Add(new ConfigurationPrivateKeyProvider(configSection));
            }

            return result;
        }
    }
}
#endif