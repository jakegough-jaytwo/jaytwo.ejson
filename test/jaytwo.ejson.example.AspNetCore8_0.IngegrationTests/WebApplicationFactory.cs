using System;
using System.IO;
using System.Linq;
using jaytwo.SolutionResolution;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace jaytwo.ejson.example.AspNetCore8_0.IngegrationTests
{
    public class WebApplicationFactory
        : WebApplicationFactory<example.AspNetCore8_0.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            Environment.SetEnvironmentVariable(
                variable: "EJK_3d953564513b09af30c9c9724c52770a2ffd13862710de857f5ef75e69350e52",
                value: "edadd0dc3f1765d78122f752ca5c01292916cba2e7e09fe796f5dcc2423faadd");

            var projectRootNamespace = typeof(example.AspNetCore8_0.Program).Namespace;
            var contentRoot = new SlnFileResolver().ResolvePathRelativeToSln($"examples/{projectRootNamespace}");
            builder.UseContentRoot(contentRoot);
        }
    }
}
