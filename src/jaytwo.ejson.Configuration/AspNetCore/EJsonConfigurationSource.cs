#if NETSTANDARD
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;

namespace jaytwo.ejson.Configuration.AspNetCore
{
    public class EJsonConfigurationSource : JsonConfigurationSource
    {
        public ILoggerFactory LoggerFactory { get; set; }
        public IConfigurationSection ConfigSection { get; set; }

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
#endif