using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace jaytwo.ejson.Configuration
{
    // see: https://github.com/aspnet/Extensions/blob/master/src/Configuration/Config.Json/src/JsonConfigurationProvider.cs
    public class EJsonConfigurationProvider : JsonConfigurationProvider
    {
        private readonly IEJsonCrypto _eJsonCrypto;
        
        public EJsonConfigurationProvider(EJsonConfigurationSource source)
            : this(source, new EJsonCrypto())
        {
        }

        internal EJsonConfigurationProvider(EJsonConfigurationSource source, IEJsonCrypto eJsonCrypto) 
            : base(source)
        {
            _eJsonCrypto = eJsonCrypto;
        }

        public override void Load(Stream stream)
        {
            var decryptedJson = _eJsonCrypto.GetDecryptedJson(stream);

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                streamWriter.Write(decryptedJson);
                streamWriter.Flush();
                memoryStream.Position = 0;

                base.Load(stream);
            }
        }
    }

}
