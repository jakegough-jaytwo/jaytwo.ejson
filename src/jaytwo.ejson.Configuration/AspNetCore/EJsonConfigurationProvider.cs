#if NETCORE
using System;
using System.IO;
using jaytwo.ejson.Exceptions;
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

        internal EJsonConfigurationProvider(EJsonConfigurationSource source, IEJsonCrypto? eJsonCrypto)
            : base(source)
        {
            _eJsonCrypto = eJsonCrypto ?? new EJsonCrypto();
        }

        public override void Load(Stream stream)
        {
            var source = Source as EJsonConfigurationSource;
            var logger = source?.LoggerFactory?.CreateLogger(this.GetType());
            var path = source?.Path;

            if (string.IsNullOrEmpty(path))
            {
                LogFailure(logger, string.Empty, "Empty Path");
                return;
            }

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

                logger?.LogInformation(default(EventId), "EJSON loaded from '{path}'", path);
            }
            catch (PrivateKeyNotFoundException privateKeyNotFoundException)
            {
                LogFailure(logger, path, privateKeyNotFoundException.Message);
            }
            catch (DecryptionFailedException decryptionFailedException)
            {
                LogFailure(logger, path, decryptionFailedException.Message);
            }
            catch (Exception ex)
            {
                LogFailure(logger, path, ex.Message);
            }
        }

        private static IPrivateKeyProvider GetKeyProvider(EJsonConfigurationSource? source)
        {
            var result = new DefaultPrivateKeyProvider();

            var configSection = source?.ConfigSection;
            if (configSection != null)
            {
                result.Add(new ConfigurationPrivateKeyProvider(configSection));
            }

            return result;
        }

        private void LogFailure(ILogger? logger, string path, string? additionalInformation = default)
        {
            var additionalInformationMessage = string.IsNullOrEmpty(additionalInformation)
                ? string.Empty
                : $" ({additionalInformation})";

            var message = $"Could not load EJSON from '{path}'{additionalInformationMessage}";

            logger?.LogError(default(EventId), message);
        }
    }
}
#endif
