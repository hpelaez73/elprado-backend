using ElPrado.McpApi.Contracts;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace ElPrado.Tests;

public class PropuestasV2ContractsTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public void Detalle_SerializaEnvelopeFechasNulosYColeccionesConElContratoV2()
    {
        PropuestaV2DetalleResponse detalle = new(
            Propuesta: 41251,
            Codigo: 536871234,
            Tipo: "PARCELA",
            FechaAlta: new DateOnly(2018, 3, 15),
            FechaBaja: null,
            Estado: "ACTIVA",
            Deuda: new EstadoDeudaV2("MOROSO 1", true, false, true),
            Parcela: null,
            Inhumados: Array.Empty<InhumadoV2>(),
            PropuestasAsociadas: Array.Empty<PropuestaAsociadaV2>(),
            Alertas: Array.Empty<string>());

        string json = JsonSerializer.Serialize(ApiResponse<PropuestaV2DetalleResponse>.Success(detalle), JsonOptions);

        Assert.Contains("\"succeeded\":true", json);
        Assert.Contains("\"fechaAlta\":\"2018-03-15\"", json);
        Assert.Contains("\"fechaBaja\":null", json);
        Assert.Contains("\"alDia\":false", json);
        Assert.Contains("\"inhumados\":[]", json);
        Assert.Contains("\"propuestasAsociadas\":[]", json);
        Assert.Contains("\"alertas\":[]", json);
    }

    [Fact]
    public void RecursosV2_ConservanLasPropiedadesCanonicas()
    {
        object[] recursos =
        {
            new PropuestaV2TitularesResponse(1, 0, Array.Empty<TitularV2>()),
            new PropuestaV2DeudaResponse(1, null, new TotalesDeudaV2(0, 0, 0, 0, 0, 0, 0), Array.Empty<CuentaCorrienteV2>()),
            new PropuestaV2ServiciosResponse(1, Array.Empty<HabilitacionServicioV2>(), Array.Empty<CupoServicioV2>(), Array.Empty<UtilizacionServicioV2>()),
            new PropuestaV2ContratosResponse(1, Array.Empty<ContratoV2>(), new FacturacionV2(Array.Empty<TitularFacturacionV2>())),
            new PropuestaV2ComprobantesResponse(1, Array.Empty<ComprobanteV2>(), new PaginacionV2(1, 20, 0, 0, false, false)),
            new PropuestaV2BusquedaResponse(Array.Empty<PropuestaBusquedaV2>(), new PaginacionV2(1, 20, 0, 0, false, false)),
            new PropuestaV2HistorialTitularesResponse(1, Array.Empty<HistorialTitularV2>())
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
        var response = new PropuestaV2BusquedaResponse(
            new[] { new PropuestaBusquedaV2(41251, "PARCELA", "ACTIVA", new ParcelaBusquedaV2("20750"),
                new[] { new TitularBusquedaV2("ASTURZZI ALFONSO", new DocumentoV2("DNI", "20123456")) }) },
            new PaginacionV2(1, 20, 1, 1, false, false));

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
        var response = new PropuestaV2TitularesResponse(41251, 1,
            new[] { new TitularV2(551922, 1, true, "RUBEN", new DocumentoV2("DNI", "20123456"),
                new ContactoV2(null, "341", null), new DomicilioV2("CALLE 1", null, null), null, new DateOnly(2021, 6, 20)) });

        AssertSnapshot(response, "propuesta", "cantidad", "titulares");
    }

    [Fact]
    public void Deuda_SerializaElSnapshotCanonicoConImportesDecimalesYEstadoFuncional()
    {
        var response = new PropuestaV2DeudaResponse(41251, new EstadoDeudaV2("JUDICIAL", true, false, false),
            new TotalesDeudaV2(10.50m, 1m, 2m, 0m, 0m, 13.50m, 13.50m),
            new[] { new CuentaCorrienteV2(7, "CUOTA", null, new EstadoDeudaV2("JUDICIAL", true, false, false),
                new ClienteReferenciaV2(1, "CLIENTE"), 13.50m, null, new TotalesDeudaV2(10.50m, 1m, 2m, 0m, 0m, 13.50m, 13.50m), null) });

        JsonObject root = AssertSnapshot(response, "propuesta", "estado", "totales", "cuentas");
        Assert.Equal(false, root["estado"]?["alDia"]?.GetValue<bool>());
        Assert.Equal(13.50m, root["cuentas"]?[0]?["importe"]?.GetValue<decimal>());
    }

    [Fact]
    public void Servicios_SerializaElSnapshotCanonicoConMotivoYColecciones()
    {
        var response = new PropuestaV2ServiciosResponse(41251,
            new[] { new HabilitacionServicioV2(null, "PLAN", "SEPELIO", false, new MotivoHabilitacionV2("MORA", "Sin servicio por mora", null)) },
            new[] { new CupoServicioV2("PLAN", "CREMACION", 3, 1, 2) },
            new[] { new UtilizacionServicioV2(new DateOnly(2024, 1, 1), "PLAN", "SEPELIO", null, null, 41251, null) });

        AssertSnapshot(response, "propuesta", "habilitaciones", "cupos", "utilizaciones");
    }

    [Fact]
    public void Contratos_SerializaElSnapshotCanonicoConPlanNuloCuandoNoHayRelacion()
    {
        var response = new PropuestaV2ContratosResponse(41251,
            new[] { new ContratoV2(2, new DateOnly(2018, 3, 15), "VENTA", "MODELO", "ACTIVO", 130000m, new VendedorV2("VENDEDOR"), null) },
            new FacturacionV2(Array.Empty<TitularFacturacionV2>()));

        JsonObject root = AssertSnapshot(response, "propuesta", "contratos", "facturacion");
        Assert.Null(root["contratos"]?[0]?["planVenta"]);
    }

    [Fact]
    public void ComprobantesEHistorial_SerializanLosSnapshotsCanonicos()
    {
        var comprobantes = new PropuestaV2ComprobantesResponse(41251,
            new[] { new ComprobanteV2(new DateOnly(2026, 8, 10), "FACTURA", "B-0006-1", new ClienteReferenciaV2(1, "CLIENTE"), 45000m, 0m, "PENDIENTE", false) },
            new PaginacionV2(1, 20, 1, 1, false, false));
        var historial = new PropuestaV2HistorialTitularesResponse(41251,
            new[] { new HistorialTitularV2(new ClienteReferenciaV2(1, "CLIENTE"), new DateOnly(2018, 3, 15), null, "ADMIN", null) });

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
