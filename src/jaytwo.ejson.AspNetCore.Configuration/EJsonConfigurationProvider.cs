using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace jaytwo.ejson.AspNetCore.Configuration
{
    // see: https://github.com/aspnet/Extensions/blob/master/src/Configuration/Config.Json/src/JsonConfigurationProvider.cs
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
            }
            catch (Exception exception)
            {
                logger?.LogError(default(EventId), exception, "Could not load EJSON from {path}", path);
            }

            logger?.LogInformation(default(EventId), "EJSON loaded from {path}", path);
        }

        private static IPrivateKeyProvider GetKeyProvider(EJsonConfigurationSource source)
        {
            var result = new DefaultPrivateKeyProvider();

            var configSection = (source)?.PrivateKeyConfigSection;
            if (configSection != null)
            {
                result.Add(new ConfigurationPrivateKeyProvider(configSection));
            }

            return result;
        }
    }

}
