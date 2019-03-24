using System;
using System.Collections.Generic;
using System.Text;
using jaytwo.ejson.AspNetCore.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.Extensions.Configuration
{
    // https://github.com/aspnet/Configuration/blob/release/1.1/src/Microsoft.Extensions.Configuration.Json/JsonConfigurationExtensions.cs
    // https://github.com/aspnet/Extensions/blob/release/2.2/src/Configuration/Config.Json/src/JsonConfigurationExtensions.cs

    public static class EJsonConfigurationExtensions
    {
        /// <summary>
        /// Adds the EJSON configuration provider from appsecrets.ejson and appsecrets.{EnvironmentName}.ejson to <paramref name="builder"/>.
        /// </summary>
        public static IConfigurationBuilder AddAppSecretsEJson(this IConfigurationBuilder builder, IHostingEnvironment hostingEnvironment)
        {
            var privateKeyConfigSectionOrDefault = builder.Build().GetSection("ejson");

            builder.AddEJsonFile("appsecrets.json",
                privateKeyConfigSection: privateKeyConfigSectionOrDefault);

            builder.AddEJsonFile($"appsecrets.{hostingEnvironment.EnvironmentName}.json",
                optional: true,
                privateKeyConfigSection: privateKeyConfigSectionOrDefault);

            return builder;
        }

        /// <summary>
        /// Adds the EJSON configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="privateKeyConfigSection">Configuration section containing private keys.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddEJsonFile(this IConfigurationBuilder builder, string path, bool optional = false, IConfigurationSection privateKeyConfigSection = null)
        {
            return AddEJsonFile(builder, provider: null, path: path, optional: optional, privateKeyConfigSection: privateKeyConfigSection);
        }

#if NETSTANDARD2_0
        /// <summary>
        /// Adds a EJSON configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="provider">The <see cref="IFileProvider"/> to use to access the file.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="privateKeyConfigSection">Configuration section containing private keys.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddEJsonFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, IConfigurationSection privateKeyConfigSection)
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
                s.PrivateKeyConfigSection = privateKeyConfigSection;
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

#if NETSTANDARD1_6
        /// <summary>
        /// Adds a EJSON configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="provider">The <see cref="IFileProvider"/> to use to access the file.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="privateKeyConfigSection">Configuration section containing private keys.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddEJsonFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, IConfigurationSection privateKeyConfigSection = null)
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
                PrivateKeyConfigSection = privateKeyConfigSection,
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