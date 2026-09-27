using ElPrado.Dto.Dtos;
using ElPrado.McpApi.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace ElPrado.Tests;

public sealed class PropuestasV2IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const int PropuestaDePrueba = 41000;
    private readonly WebApplicationFactory<Program> _factory;

    public PropuestasV2IntegrationTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Theory]
    [InlineData("")]
    [InlineData("/titulares")]
    [InlineData("/deuda")]
    [InlineData("/servicios")]
    [InlineData("/contratos")]
    [InlineData("/comprobantes?page=1&pageSize=2")]
    [InlineData("/historial-titulares")]
    public async Task RutasV2_Autenticadas_DevuelvenEnvelopeCanonico(string suffix)
    {
        using HttpClient client = CreateAuthenticatedClient();
        HttpResponseMessage response = await client.GetAsync($"/api/v2/propuestas/{PropuestaDePrueba}{suffix}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(body.RootElement.GetProperty("succeeded").GetBoolean());
        Assert.Equal(JsonValueKind.Object, body.RootElement.GetProperty("data").ValueKind);
        Assert.Equal(JsonValueKind.Null, body.RootElement.GetProperty("error").ValueKind);
    }

    [Fact]
    public async Task RutaV2_SinAutenticacion_RechazaLaSolicitud()
    {
        using HttpClient client = _factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync($"/api/v2/propuestas/{PropuestaDePrueba}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/v2/propuestas/0", HttpStatusCode.BadRequest, "INVALID_REQUEST")]
    [InlineData("/api/v2/propuestas/999999999", HttpStatusCode.NotFound, "PROPOSAL_NOT_FOUND")]
    [InlineData("/api/v2/propuestas/41000/comprobantes?page=0", HttpStatusCode.BadRequest, "INVALID_REQUEST")]
    public async Task RutaV2_ConSolicitudInvalidaONoEncontrada_DevuelveErrorEstable(string route, HttpStatusCode expectedStatus, string expectedCode)
    {
        using HttpClient client = CreateAuthenticatedClient();
        HttpResponseMessage response = await client.GetAsync(route);

        Assert.Equal(expectedStatus, response.StatusCode);
        using JsonDocument body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.False(body.RootElement.GetProperty("succeeded").GetBoolean());
        Assert.Equal(JsonValueKind.Null, body.RootElement.GetProperty("data").ValueKind);
        Assert.Equal(expectedCode, body.RootElement.GetProperty("error").GetProperty("code").GetString());
    }

    [Fact]
    public async Task ComprobantesV2_RespetaLaPaginacionSolicitada()
    {
        using HttpClient client = CreateAuthenticatedClient();
        HttpResponseMessage response = await client.GetAsync($"/api/v2/propuestas/{PropuestaDePrueba}/comprobantes?page=1&pageSize=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        JsonElement pagination = body.RootElement.GetProperty("data").GetProperty("pagination");
        Assert.Equal(1, pagination.GetProperty("page").GetInt32());
        Assert.Equal(2, pagination.GetProperty("pageSize").GetInt32());
        Assert.True(pagination.TryGetProperty("totalItems", out _));
        Assert.True(pagination.TryGetProperty("totalPages", out _));
        Assert.True(pagination.TryGetProperty("hasNext", out _));
        Assert.True(pagination.TryGetProperty("hasPrevious", out _));
    }

    private HttpClient CreateAuthenticatedClient()
    {
        using IServiceScope scope = _factory.Services.CreateScope();
        McpJwtTokenIssuer issuer = scope.ServiceProvider.GetRequiredService<McpJwtTokenIssuer>();
        string token = issuer.Issue(new DtoLogin { CodUsuario = 1, Nombre = "Integracion" }).Token;
        HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
