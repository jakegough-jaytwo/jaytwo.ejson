#if NETSTANDARD
using System;
using System.Collections.Generic;
using System.Text;
using jaytwo.ejson.Configuration.AspNetCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Microsoft.Extensions.Configuration
{
    // inspired from:
    // https://github.com/aspnet/Configuration/blob/release/1.1/src/Microsoft.Extensions.Configuration.Json/JsonConfigurationExtensions.cs
    // https://github.com/aspnet/Extensions/blob/release/2.2/src/Configuration/Config.Json/src/JsonConfigurationExtensions.cs

    public static class EJsonConfigurationExtensions
    {
        /// <summary>
        /// Adds the EJSON configuration provider from appsecrets.ejson and appsecrets.{env.EnvironmentName}.ejson to <paramref name="builder"/>.
        /// </summary>
        public static IConfigurationBuilder AddEjsonAppSecrets(this IConfigurationBuilder builder, IHostingEnvironment env = null, ILoggerFactory loggerFactory = null, IConfigurationSection configSection = null)
        {
            if (!builder.Properties.TryGetValue("FileProvider", out object handler))
            {
                builder.SetBasePath(env.ContentRootPath);
            }

            builder.AddEJsonFile("appsecrets.json",
                configSection: configSection,
                loggerFactory: loggerFactory);

            if (env != null)
            {
                builder.AddEJsonFile($"appsecrets.{env.EnvironmentName}.json",
                    optional: true,
                    configSection: configSection,
                    loggerFactory: loggerFactory);
            }

            return builder;
        }

        /// <summary>
        /// Adds the EJSON configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="configSection">Configuration section containing private keys.</param>
        /// <param name="loggerFactory">(Optional) Log target.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddEJsonFile(this IConfigurationBuilder builder, string path, bool optional = false, IConfigurationSection configSection = null, ILoggerFactory loggerFactory = null)
        {
            return AddEJsonFile(builder, provider: null, path: path, optional: optional, configSection: configSection, loggerFactory: loggerFactory);
        }

#if NETSTANDARD2
        /// <summary>
        /// Adds a EJSON configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="provider">The <see cref="IFileProvider"/> to use to access the file.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="configSection">(Optional) Configuration section containing private keys.</param>
        /// <param name="loggerFactory">(Optional) Log target.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddEJsonFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, IConfigurationSection configSection, ILoggerFactory loggerFactory)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("File path must be a non-empty string.", nameof(path));
            }

            return builder.AddEJsonFile(s =>
            {
                s.ConfigSection = configSection;
                s.LoggerFactory = loggerFactory;
                s.FileProvider = provider;
                s.Path = path;
                s.Optional = optional;
                s.ResolveFileProvider();
            });
        }

        /// <summary>
        /// Adds a EJSON configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddEJsonFile(this IConfigurationBuilder builder, Action<EJsonConfigurationSource> configureSource) => builder.Add(configureSource);
#endif

#if NETSTANDARD1
        /// <summary>
        /// Adds a EJSON configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="provider">The <see cref="IFileProvider"/> to use to access the file.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="configSection">Configuration section containing private keys.</param>
        /// <param name="loggerFactory">(Optional) Log target.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddEJsonFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, IConfigurationSection configSection, ILoggerFactory loggerFactory)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("File path must be a non-empty string.", nameof(path));
            }

            var source = new EJsonConfigurationSource
            {
                ConfigSection = configSection,
                LoggerFactory = loggerFactory,
                FileProvider = provider,
                Path = path,
                Optional = optional,
                ReloadOnChange = false,
            };

            source.ResolveFileProvider();
            builder.Add(source);
            return builder;
        }
#endif
    }
}
#endif
