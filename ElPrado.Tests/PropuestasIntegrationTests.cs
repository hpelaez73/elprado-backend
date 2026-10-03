using ElPrado.Dto.Dtos;
using ElPrado.McpApi.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace ElPrado.Tests;

public sealed class PropuestasIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const int PropuestaDePrueba = 41000;
    private readonly WebApplicationFactory<Program> _factory;

    public PropuestasIntegrationTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Theory]
    [InlineData("")]
    [InlineData("/titulares")]
    [InlineData("/deuda")]
    [InlineData("/servicios")]
    [InlineData("/contratos")]
    [InlineData("/comprobantes?page=1&pageSize=2")]
    [InlineData("/historial-titulares")]
    public async Task Rutas_Autenticadas_DevuelvenEnvelopeCanonico(string suffix)
    {
        using HttpClient client = CreateAuthenticatedClient();
        HttpResponseMessage response = await client.GetAsync($"/api/propuestas/{PropuestaDePrueba}{suffix}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(body.RootElement.GetProperty("succeeded").GetBoolean());
        Assert.Equal(JsonValueKind.Object, body.RootElement.GetProperty("data").ValueKind);
        Assert.Equal(JsonValueKind.Null, body.RootElement.GetProperty("error").ValueKind);
    }

    [Fact]
    public async Task Ruta_SinAutenticacion_RechazaLaSolicitud()
    {
        using HttpClient client = _factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync($"/api/propuestas/{PropuestaDePrueba}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Busqueda_SinAutenticacion_RechazaLaSolicitud()
    {
        using HttpClient client = _factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync("/api/propuestas/buscar?nombre=asturzzi");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/propuestas/0", HttpStatusCode.BadRequest, "INVALID_REQUEST")]
    [InlineData("/api/propuestas/999999999", HttpStatusCode.NotFound, "PROPOSAL_NOT_FOUND")]
    [InlineData("/api/propuestas/41000/comprobantes?page=0", HttpStatusCode.BadRequest, "INVALID_REQUEST")]
    [InlineData("/api/propuestas/buscar", HttpStatusCode.BadRequest, "INVALID_REQUEST")]
    [InlineData("/api/propuestas/buscar?nombre=asturzzi&pageSize=101", HttpStatusCode.BadRequest, "INVALID_REQUEST")]
    [InlineData("/api/propuestas/buscar?documento=no-es-documento", HttpStatusCode.BadRequest, "INVALID_REQUEST")]
    public async Task Ruta_ConSolicitudInvalidaONoEncontrada_DevuelveErrorEstable(string route, HttpStatusCode expectedStatus, string expectedCode)
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
    public async Task Comprobantes_RespetaLaPaginacionSolicitada()
    {
        using HttpClient client = CreateAuthenticatedClient();
        HttpResponseMessage response = await client.GetAsync($"/api/propuestas/{PropuestaDePrueba}/comprobantes?page=1&pageSize=2");

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

    [Fact]
    public async Task Busqueda_EncuentraLaPropuestaPorNombreYDocumento()
    {
        using HttpClient client = CreateAuthenticatedClient();
        HttpResponseMessage titularesResponse = await client.GetAsync($"/api/propuestas/{PropuestaDePrueba}/titulares");
        Assert.Equal(HttpStatusCode.OK, titularesResponse.StatusCode);

        using JsonDocument titularesBody = JsonDocument.Parse(await titularesResponse.Content.ReadAsStringAsync());
        JsonElement titular = titularesBody.RootElement.GetProperty("data").GetProperty("titulares")[0];
        string nombre = titular.GetProperty("nombre").GetString()!;
        string documento = titular.GetProperty("documento").GetProperty("numero").GetString()!;

        HttpResponseMessage response = await client.GetAsync($"/api/propuestas/buscar?nombre={Uri.EscapeDataString(nombre)}&documento={Uri.EscapeDataString(documento)}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        JsonElement data = body.RootElement.GetProperty("data");
        JsonElement propuesta = data.GetProperty("items").EnumerateArray().Single(x => x.GetProperty("propuesta").GetInt32() == PropuestaDePrueba);
        Assert.True(data.GetProperty("pagination").TryGetProperty("totalItems", out _));

        if (propuesta.TryGetProperty("parcela", out JsonElement parcela) && parcela.ValueKind == JsonValueKind.Object)
        {
            string numeroParcela = parcela.GetProperty("numero").GetString()!;
            HttpResponseMessage parcelaResponse = await client.GetAsync($"/api/propuestas/buscar?parcela={Uri.EscapeDataString(numeroParcela)}");
            Assert.Equal(HttpStatusCode.OK, parcelaResponse.StatusCode);
            using JsonDocument parcelaBody = JsonDocument.Parse(await parcelaResponse.Content.ReadAsStringAsync());
            Assert.Contains(parcelaBody.RootElement.GetProperty("data").GetProperty("items").EnumerateArray(), x => x.GetProperty("propuesta").GetInt32() == PropuestaDePrueba);
        }
    }

    [Fact]
    public async Task Busqueda_SinCoincidencias_DevuelvePaginaVacia()
    {
        using HttpClient client = CreateAuthenticatedClient();
        HttpResponseMessage response = await client.GetAsync("/api/propuestas/buscar?nombre=zzqvwxk");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using JsonDocument body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        JsonElement data = body.RootElement.GetProperty("data");
        Assert.Equal(JsonValueKind.Array, data.GetProperty("items").ValueKind);
        Assert.Equal(0, data.GetProperty("items").GetArrayLength());
        Assert.Equal(0, data.GetProperty("pagination").GetProperty("totalItems").GetInt32());
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
