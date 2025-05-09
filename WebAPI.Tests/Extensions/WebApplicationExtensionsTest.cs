using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Threading.Tasks;
using System.Net;

public class MiddlewareTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public MiddlewareTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Swagger_Endpoint_Should_Work_In_Dev()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/swagger/index.html");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}



