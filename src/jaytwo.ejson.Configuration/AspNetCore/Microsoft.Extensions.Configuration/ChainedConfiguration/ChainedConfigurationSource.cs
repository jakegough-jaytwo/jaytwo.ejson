// https://github.com/aspnet/Configuration/blob/master/src/Config/ChainedConfigurationSource.cs
// TODO: figure out if this is kosher to include

#if NETSTANDARD1_3 || NETSTANDARD1_6

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Represents a chained IConfiguration as an <see cref="IConfigurationSource"/>.
    /// </summary>
    public class ChainedConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// Gets or sets the chained configuration.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Builds the <see cref="ChainedConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="ChainedConfigurationProvider"/>.</returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
            => new ChainedConfigurationProvider(this);
    }
}

#endif
