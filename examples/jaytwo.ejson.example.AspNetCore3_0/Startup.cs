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

        public Startup(IConfiguration configurationBeforeSecrets, IWebHostEnvironment env)
        {
            var environmentName = env.EnvironmentName;

            using (var loggerFactory = GetEarlyInitializationLoggerFactory())
            {
                _configuration = new ConfigurationBuilder()
                    .AddConfiguration(configurationBeforeSecrets)
                    .AddJsonFile("appsettings.json")
                    .AddEJsonFile($"appsecrets.json", optional: false, loggerFactory: loggerFactory)
                    .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                    .AddEJsonFile($"appsecrets.{environmentName}.json", optional: true, loggerFactory: loggerFactory)
                    .Build();
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

        private static ILoggerFactory GetEarlyInitializationLoggerFactory()
        {
            return LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
        }
    }
}
