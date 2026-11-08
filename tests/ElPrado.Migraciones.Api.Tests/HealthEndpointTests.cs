using System.Net;
using Xunit;

namespace ElPrado.Migraciones.Api.Tests;

public sealed class HealthEndpointTests : IClassFixture<MigrationApiFactory>
{
    private readonly MigrationApiFactory _factory;

    public HealthEndpointTests(MigrationApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public void CreateClient_initializes_the_application_without_external_connections()
    {
        using var client = _factory.CreateClient();

        Assert.NotNull(client);
    }

    [Fact]
    public async Task Get_health_returns_ok_when_the_application_is_healthy()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_health_is_available_without_authentication()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(client.DefaultRequestHeaders.Authorization);
    }
}
