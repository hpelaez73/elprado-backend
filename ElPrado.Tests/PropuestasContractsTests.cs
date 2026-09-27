using ElPrado.McpApi.Contracts;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace ElPrado.Tests;

public class PropuestasContractsTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public void Detalle_SerializaEnvelopeFechasNulosYColeccionesConElContrato()
    {
        PropuestaDetalleResponse detalle = new(
            Propuesta: 41251,
            Codigo: 536871234,
            Tipo: "PARCELA",
            FechaAlta: new DateOnly(2018, 3, 15),
            FechaBaja: null,
            Estado: "ACTIVA",
            Deuda: new EstadoDeuda("MOROSO 1", true, false, true),
            Parcela: null,
            Inhumados: Array.Empty<Inhumado>(),
            PropuestasAsociadas: Array.Empty<PropuestaAsociada>(),
            Alertas: Array.Empty<string>());

        string json = JsonSerializer.Serialize(ApiResponse<PropuestaDetalleResponse>.Success(detalle), JsonOptions);

        Assert.Contains("\"succeeded\":true", json);
        Assert.Contains("\"fechaAlta\":\"2018-03-15\"", json);
        Assert.Contains("\"fechaBaja\":null", json);
        Assert.Contains("\"alDia\":false", json);
        Assert.Contains("\"inhumados\":[]", json);
        Assert.Contains("\"propuestasAsociadas\":[]", json);
        Assert.Contains("\"alertas\":[]", json);
    }

    [Fact]
    public void Recursos_ConservanLasPropiedadesCanonicas()
    {
        object[] recursos =
        {
            new PropuestaTitularesResponse(1, 0, Array.Empty<Titular>()),
            new PropuestaDeudaResponse(1, null, new TotalesDeuda(0, 0, 0, 0, 0, 0, 0), Array.Empty<CuentaCorriente>()),
            new PropuestaServiciosResponse(1, Array.Empty<HabilitacionServicio>(), Array.Empty<CupoServicio>(), Array.Empty<UtilizacionServicio>()),
            new PropuestaContratosResponse(1, Array.Empty<Contrato>(), new Facturacion(Array.Empty<TitularFacturacion>())),
            new PropuestaComprobantesResponse(1, Array.Empty<Comprobante>(), new Paginacion(1, 20, 0, 0, false, false)),
            new PropuestaBusquedaResponse(Array.Empty<PropuestaBusqueda>(), new Paginacion(1, 20, 0, 0, false, false)),
            new PropuestaHistorialTitularesResponse(1, Array.Empty<HistorialTitular>())
        };

        string json = JsonSerializer.Serialize(recursos, JsonOptions);

        Assert.Contains("\"titulares\":[]", json);
        Assert.Contains("\"totales\":{", json);
        Assert.Contains("\"habilitaciones\":[]", json);
        Assert.Contains("\"facturacion\":{", json);
        Assert.Contains("\"pagination\":{", json);
        Assert.Contains("\"historial\":[]", json);
    }

    [Fact]
    public void Busqueda_SerializaElContratoCompactoSinCodigosInternos()
    {
        var response = new PropuestaBusquedaResponse(
            new[] { new PropuestaBusqueda(41251, "PARCELA", "ACTIVA", new ParcelaBusqueda("20750"),
                new[] { new TitularBusqueda("ASTURZZI ALFONSO", new Documento("DNI", "20123456")) }) },
            new Paginacion(1, 20, 1, 1, false, false));

        JsonObject root = AssertSnapshot(response, "items", "pagination");
        JsonObject item = root["items"]![0]!.AsObject();
        Assert.Equal(41251, item["propuesta"]!.GetValue<int>());
        Assert.False(item.ContainsKey("codPropuesta"));
        Assert.False(item.ContainsKey("codigo"));
        Assert.Equal("20750", item["parcela"]?["numero"]?.GetValue<string>());
    }

    [Fact]
    public void Titulares_SerializaElSnapshotCanonico()
    {
        var response = new PropuestaTitularesResponse(41251, 1,
            new[] { new Titular(551922, 1, true, "RUBEN", new Documento("DNI", "20123456"),
                new Contacto(null, "341", null), new Domicilio("CALLE 1", null, null), null, new DateOnly(2021, 6, 20)) });

        AssertSnapshot(response, "propuesta", "cantidad", "titulares");
    }

    [Fact]
    public void Deuda_SerializaElSnapshotCanonicoConImportesDecimalesYEstadoFuncional()
    {
        var response = new PropuestaDeudaResponse(41251, new EstadoDeuda("JUDICIAL", true, false, false),
            new TotalesDeuda(10.50m, 1m, 2m, 0m, 0m, 13.50m, 13.50m),
            new[] { new CuentaCorriente(7, "CUOTA", null, new EstadoDeuda("JUDICIAL", true, false, false),
                new ClienteReferencia(1, "CLIENTE"), 13.50m, null, new TotalesDeuda(10.50m, 1m, 2m, 0m, 0m, 13.50m, 13.50m), null) });

        JsonObject root = AssertSnapshot(response, "propuesta", "estado", "totales", "cuentas");
        Assert.Equal(false, root["estado"]?["alDia"]?.GetValue<bool>());
        Assert.Equal(13.50m, root["cuentas"]?[0]?["importe"]?.GetValue<decimal>());
    }

    [Fact]
    public void Servicios_SerializaElSnapshotCanonicoConMotivoYColecciones()
    {
        var response = new PropuestaServiciosResponse(41251,
            new[] { new HabilitacionServicio(null, "PLAN", "SEPELIO", false, new MotivoHabilitacion("MORA", "Sin servicio por mora", null)) },
            new[] { new CupoServicio("PLAN", "CREMACION", 3, 1, 2) },
            new[] { new UtilizacionServicio(new DateOnly(2024, 1, 1), "PLAN", "SEPELIO", null, null, 41251, null) });

        AssertSnapshot(response, "propuesta", "habilitaciones", "cupos", "utilizaciones");
    }

    [Fact]
    public void Contratos_SerializaElSnapshotCanonicoConPlanNuloCuandoNoHayRelacion()
    {
        var response = new PropuestaContratosResponse(41251,
            new[] { new Contrato(2, new DateOnly(2018, 3, 15), "VENTA", "MODELO", "ACTIVO", 130000m, new Vendedor("VENDEDOR"), null) },
            new Facturacion(Array.Empty<TitularFacturacion>()));

        JsonObject root = AssertSnapshot(response, "propuesta", "contratos", "facturacion");
        Assert.Null(root["contratos"]?[0]?["planVenta"]);
    }

    [Fact]
    public void ComprobantesEHistorial_SerializanLosSnapshotsCanonicos()
    {
        var comprobantes = new PropuestaComprobantesResponse(41251,
            new[] { new Comprobante(new DateOnly(2026, 8, 10), "FACTURA", "B-0006-1", new ClienteReferencia(1, "CLIENTE"), 45000m, 0m, "PENDIENTE", false) },
            new Paginacion(1, 20, 1, 1, false, false));
        var historial = new PropuestaHistorialTitularesResponse(41251,
            new[] { new HistorialTitular(new ClienteReferencia(1, "CLIENTE"), new DateOnly(2018, 3, 15), null, "ADMIN", null) });

        AssertSnapshot(comprobantes, "propuesta", "items", "pagination");
        AssertSnapshot(historial, "propuesta", "historial");
    }

    private static JsonObject AssertSnapshot<T>(T response, params string[] properties)
    {
        string json = JsonSerializer.Serialize(ApiResponse<T>.Success(response), JsonOptions);
        JsonObject root = JsonNode.Parse(json)!.AsObject();
        JsonObject data = root["data"]!.AsObject();

        Assert.True(root["succeeded"]!.GetValue<bool>());
        Assert.Null(root["error"]);
        foreach (string property in properties)
            Assert.True(data.ContainsKey(property), $"Falta la propiedad canónica '{property}'. JSON: {json}");

        return data;
    }
}
