using Afip.Data;
using Afip.Data.Factories;
using Afip.Data.Interfaces;
using Afip.Data.Models;
using Afip.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSingleton<IDbConnectionFactory, FirebirdConnectionFactory>();
builder.Services.AddSingleton<IAfipRepository, AfipRepository>();
builder.Services.AddTransient<Wsfe>();

var app = builder.Build();

app.MapGet("/api/wsfe/estado-servicio", (Wsfe wsfe) =>
{
    ApiResponse<object> res = wsfe.ConsultarEstadoServicio();
    return res.Success ? Results.Ok(res) : Results.BadRequest(res);
});

app.MapGet("/api/wsfe/ultimo-comprobante/{codTipoComprobante:int}/{codTalonario:int}", async (int codTipoComprobante, int codTalonario, Wsfe wsfe) =>
{
    ApiResponse<object> res = await wsfe.ConsultarUltimoComprobanteAsync(codTipoComprobante, codTalonario);
    return res.Success ? Results.Ok(res) : Results.BadRequest(res);
});

app.MapGet("/api/wsfe/comprobante/{codTalonario:int}/{nroComprobante}", async (int codTalonario, string nroComprobante, Wsfe wsfe) =>
{
    ApiResponse<ElPrado.Dto.Dtos.DtoAfipWsfeConsultaDetalle> res = await wsfe.ConsultarComprobanteAsync(codTalonario, nroComprobante);
    return res.Success ? Results.Ok(res) : Results.BadRequest(res);
});

app.MapPost("/api/wsfe/actualizar-cae/{codTalonario:int}/{nroComprobante}", async (int codTalonario, string nroComprobante, Wsfe wsfe) =>
{
    ApiResponse<object> res = await wsfe.ActualizarCAEComprobanteEmitidoAsync(codTalonario, nroComprobante);
    return res.Success ? Results.Ok(res) : Results.BadRequest(res);
});

app.MapPost("/api/wsfe/solicitar-cae/{codTalonario:int}/{nroComprobante}", async (int codTalonario, string nroComprobante, Wsfe wsfe) =>
{
    ApiResponse<object> res = await wsfe.SolicitarCAEAsync(codTalonario, nroComprobante);
    return res.Success ? Results.Ok(res) : Results.BadRequest(res);
});

app.Run();
