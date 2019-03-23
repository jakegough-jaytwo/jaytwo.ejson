using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace jaytwo.ejson.Configuration
{
    public class EJsonConfigurationSource : JsonConfigurationSource
    {
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
