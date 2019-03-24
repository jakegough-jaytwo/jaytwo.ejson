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
            try
            {
                var privateKeyProvider = GetKeyProvider();
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
                // TODO: figure out what order to initialize things to make this logging output something

                var path = Source.Path;

                new LoggerFactory()
                    .CreateLogger<EJsonConfigurationProvider>()
                    .LogError(new EventId(), exception, "Could not load EJSON from {path}", path);
            }
        }

        private IPrivateKeyProvider GetKeyProvider()
        {
            var result = new DefaultPrivateKeyProvider();

            var configSection = (Source as EJsonConfigurationSource)?.PrivateKeyConfigSection;
            if (configSection != null)
            {
                result.Add(new ConfigurationPrivateKeyProvider(configSection));
            }

            return result;
        }
    }

}
