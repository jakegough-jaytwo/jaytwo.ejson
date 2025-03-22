using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace jaytwo.ejson.example.AspNetCore8_0;

public class Program
{
    public static int Main(string[] args)
    {
        try
        {
            using (var app = CreateAppBuilder(args).Build())
            {
                app.MapControllers();
                app.Run();
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    public static WebApplicationBuilder CreateAppBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var environmentName = builder.Environment.EnvironmentName;

        using (var loggerFactory = GetEarlyInitializationLoggerFactory())
        {
            builder.Configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEJsonFile($"appsecrets.json", optional: false, loggerFactory: loggerFactory)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEJsonFile($"appsecrets.{environmentName}.json", optional: true, loggerFactory: loggerFactory)
                .AddEnvironmentVariables();
        }

        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
            });

        // DI Here

        return builder;
    }

    private static ILoggerFactory GetEarlyInitializationLoggerFactory()
    {
        return LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
    }
}
