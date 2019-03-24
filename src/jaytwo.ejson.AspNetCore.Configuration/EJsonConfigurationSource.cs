using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;

namespace jaytwo.ejson.AspNetCore.Configuration
{
    public class EJsonConfigurationSource : JsonConfigurationSource
    {
        public ILoggerFactory LoggerFactory { get; set; }
        public IConfigurationSection PrivateKeyConfigSection { get; set; }

        public EJsonConfigurationSource()
        {
        }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new EJsonConfigurationProvider(this);
        }
    }
}
