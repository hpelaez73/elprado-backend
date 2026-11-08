using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ElPrado.Migraciones.Api.Tests;

public sealed class AuthenticationEndpointTests : IClassFixture<MigrationApiFactory>
{
    private readonly MigrationApiFactory _factory;
    public AuthenticationEndpointTests(MigrationApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Admin_login_and_me_return_minimal_identity()
    {
        using var client = _factory.CreateClient();
        var login = await client.PostAsJsonAsync("/auth/admin/login", new { username = "admin", password = "test-password" });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var token = await login.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(token?.AccessToken);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);
        var me = await client.GetAsync("/auth/me");
        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
    }

    [Fact]
    public async Task Invalid_agent_credential_returns_generic_unauthorized()
    {
        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/auth/agent/token", new { agentId = "agent-one", agentSecret = "wrong" });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Agent_token_cannot_access_admin_identity()
    {
        using var client = _factory.CreateClient();
        var login = await client.PostAsJsonAsync("/auth/agent/token", new { agentId = "agent-one", agentSecret = "test-password" });
        var token = await login.Content.ReadFromJsonAsync<TokenResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);
        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/auth/me")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/auth/agent/me")).StatusCode);
    }

    [Fact]
    public async Task Missing_or_tampered_token_is_rejected()
    {
        using var client = _factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/auth/me")).StatusCode);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not-a-jwt");
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/auth/me")).StatusCode);
    }

    [Fact]
    public async Task Agents_have_independent_identity()
    {
        using var client = _factory.CreateClient();
        var first = await GetAgentToken(client, "agent-one");
        var second = await GetAgentToken(client, "agent-two");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", first);
        Assert.Contains("agent-one", await (await client.GetAsync("/auth/agent/me")).Content.ReadAsStringAsync());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", second);
        Assert.Contains("agent-two", await (await client.GetAsync("/auth/agent/me")).Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Http_authentication_is_rejected_when_not_explicitly_allowed()
    {
        using var factory = _factory.WithWebHostBuilder(builder => builder.ConfigureAppConfiguration(config => config.AddInMemoryCollection(new Dictionary<string, string?> { ["MigrationAuthentication:AllowInsecureLocalRequests"] = "false" })));
        using var client = factory.CreateClient();
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/auth/admin/login", new { username = "admin", password = "test-password" })).StatusCode);
    }

    [Fact]
    public async Task Token_rate_limit_rejects_before_another_validation()
    {
        using var factory = _factory.WithWebHostBuilder(builder => builder.ConfigureAppConfiguration(config => config.AddInMemoryCollection(new Dictionary<string, string?> { ["MigrationAuthentication:RateLimit:PermitLimit"] = "1" })));
        using var client = factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.PostAsJsonAsync("/auth/admin/login", new { username = "admin", password = "wrong" })).StatusCode);
        Assert.Equal((HttpStatusCode)429, (await client.PostAsJsonAsync("/auth/admin/login", new { username = "admin", password = "test-password" })).StatusCode);
    }

    private static async Task<string> GetAgentToken(HttpClient client, string agentId)
    {
        var response = await client.PostAsJsonAsync("/auth/agent/token", new { agentId, agentSecret = "test-password" });
        return (await response.Content.ReadFromJsonAsync<TokenResponse>())!.AccessToken;
    }

    private sealed record TokenResponse(string AccessToken, DateTimeOffset ExpiresAt);
}
