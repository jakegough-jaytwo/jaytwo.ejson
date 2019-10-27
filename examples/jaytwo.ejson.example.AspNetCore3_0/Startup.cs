using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace jaytwo.ejson.example.AspNetCore3_0
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public Startup(IConfiguration configurationBeforeSecrets, IWebHostEnvironment env)
        {
            _logger = GetEarlyInitializationLogger();

            /*
             * AddEjsonAppSecrets() is an opinionated extension method to setup EJSON app secrets.  It will:
             *   a) call `builder.SetBasePath(Directory.GetCurrentDirectory())` if not previously set
             *   b) configure files `appsecrets.json` and `appsecrets.{env}.json` for EJSON secrets
             *   c) look in multiple places for private keys, including:
             *     i)   upstream configuration (`configurationBeforeSecrets`) in a ConfigSection named 'ejson'
             *     ii)  default EJSON bheavior (filesystem, environment variables -- see EJSON readme)
             *   d) log success and failure with the optional `ILoggerFactory`
             */

            try
            {
                _configuration = new ConfigurationBuilder()
                    .AddConfiguration(configurationBeforeSecrets)
                    .AddEjsonAppSecrets(env, configSection: configurationBeforeSecrets.GetSection("ejson"))
                    .Build();

                _logger?.LogInformation(default(EventId), "Secrets loaded");
            }
            catch (Exception exception)
            {
                _logger?.LogError(default(EventId), exception, "Could not load secrets!");
                _configuration = configurationBeforeSecrets;
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // be sure to update the IConfiguration instance that's going to be injected downstream
            services.AddSingleton(x => _configuration);

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

        private ILogger GetEarlyInitializationLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            using (loggerFactory)
            {
                return loggerFactory.CreateLogger("Initialization");
            }
        }
    }
}
