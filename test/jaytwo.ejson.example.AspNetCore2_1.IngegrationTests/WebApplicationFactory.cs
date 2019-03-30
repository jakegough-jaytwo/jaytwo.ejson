using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace jaytwo.ejson.example.AspNetCore2_1.IngegrationTests
{
    public class WebApplicationFactory
        : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            Environment.SetEnvironmentVariable(
                variable: "EJK_3d953564513b09af30c9c9724c52770a2ffd13862710de857f5ef75e69350e52",
                value: "edadd0dc3f1765d78122f752ca5c01292916cba2e7e09fe796f5dcc2423faadd");

            var solutionRelativePath = GetSolutaionRelativePath("examples/jaytwo.ejson.example.AspNetCore2_1");
            builder.UseContentRoot(solutionRelativePath);
        }

        public string GetSolutaionRelativePath(string relativePath)
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            var slnPresent = false;

            while (!slnPresent && directory.Parent != null)
            {
                directory = directory.Parent;
                slnPresent = directory.GetFiles("*.sln", SearchOption.TopDirectoryOnly).Any();
            }

            if (slnPresent)
            {
                var path = Path.Combine(directory.FullName, relativePath);
                return new DirectoryInfo(path).FullName;
            }

            throw new InvalidOperationException("Could not find solution file!");
        }
    }
}
