using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using jaytwo.SolutionResolution;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace jaytwo.ejson.example.AspNetCore1_1.IngegrationTests
{
    public class TestServerFixture
    {
        private readonly TestServer _server;

        public TestServerFixture()
        {
            Environment.SetEnvironmentVariable(
                variable: "EJK_3d953564513b09af30c9c9724c52770a2ffd13862710de857f5ef75e69350e52",
                value: "edadd0dc3f1765d78122f752ca5c01292916cba2e7e09fe796f5dcc2423faadd");

            var contentRoot = new SlnFileResolver().ResolvePathRelativeToSln("examples/jaytwo.ejson.example.AspNetCore1_1");

            _server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseContentRoot(contentRoot)
                .UseStartup<Startup>());
        }

        public HttpClient CreateClient() => _server.CreateClient();

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}
