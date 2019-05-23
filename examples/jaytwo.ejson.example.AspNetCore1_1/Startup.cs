using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace jaytwo.ejson.example.AspNetCore1_1
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var configurationBeforeSecrets = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .Build();

            try
            {
                _configuration = new ConfigurationBuilder()
                    .AddConfiguration(configurationBeforeSecrets)
                    .AddEjsonAppSecrets(env, loggerFactory, configurationBeforeSecrets.GetSection("ejson"))
                    .Build();
            }
            catch (Exception exception)
            {
                loggerFactory.CreateLogger(GetType())?.LogError(default(EventId), exception, "Could not load secrets!");
                _configuration = configurationBeforeSecrets;
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton(x => _configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();

            app.UseMvcWithDefaultRoute();
        }
    }
}
