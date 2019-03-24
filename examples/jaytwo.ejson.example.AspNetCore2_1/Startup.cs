using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace jaytwo.ejson.example.AspNetCore2_1
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configurationBeforeSecrets, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // In Startup.cs instead of Program.cs so we can have an injected ILoggerFactory that's already configured

            /* 
             * AddEjsonAppSecrets() is an opinionated extension method to setup EJSON app secrets.  It will:
             *   a) call `builder.SetBasePath(Directory.GetCurrentDirectory())` if not previously set
             *   b) configure files `appsecrets.json` and `appsecrets.{env}.json` for EJSON secrets
             *   c) look in multiple places for private keys, including:
             *     i)   upstream configuration (`configurationBeforeSecrets`) in a ConfigSection named 'ejson'
             *     ii)  default EJSON bheavior (filesystem, environment variables -- see EJSON readme)
             *   d) log success and failure with the optional `ILoggerFactory`
             */

            _configuration = new ConfigurationBuilder()
                .AddConfiguration(configurationBeforeSecrets)
                .AddEjsonAppSecrets(env, loggerFactory)
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // be sure to update the IConfiguration instance that's going to be injected downstream
            services.AddSingleton(x => _configuration);

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseMvcWithDefaultRoute();
        }
    }
}
