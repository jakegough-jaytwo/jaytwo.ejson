using System;
using System.Threading.Tasks;
using Xunit;

namespace jaytwo.ejson.example.AspNetCore3_0.IngegrationTests
{
    public class BasicTests : IClassFixture<WebApplicationFactory>
    {
        private readonly WebApplicationFactory _fixture;

        public BasicTests(WebApplicationFactory fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
        {
            // Arrange
            var client = _fixture.CreateClient();
            var expectedSecret = "rosebud was the sled";
            var expectedEnvironmentSpecificSecret = "Development";
            var expectedNamespace = typeof(example.AspNetCore3_0.Startup).Assembly.GetName().Name;

            // Act
            using (var response = await client.GetAsync("/"))
            {
                // Assert
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                Assert.Contains(expectedNamespace, content);
                Assert.Contains(expectedSecret, content);
                Assert.Contains(expectedEnvironmentSpecificSecret, content);
            }
        }
    }
}
