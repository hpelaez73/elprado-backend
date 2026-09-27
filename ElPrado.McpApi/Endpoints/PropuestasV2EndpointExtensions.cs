using ElPrado.Core;
using ElPrado.Dto.Dtos;
using ElPrado.McpApi.Contracts;
using ElPrado.Services.Services;

namespace ElPrado.McpApi.Endpoints;

/// <summary>HTTP read adapter for the public, agent-oriented proposal API v2.</summary>
public static class PropuestasV2EndpointExtensions
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    public static RouteGroupBuilder MapPropuestasV2Operations(this RouteGroupBuilder group)
    {
        RouteGroupBuilder propuestas = group.MapGroup("/v2/propuestas").WithTags("Propuestas v2");

        propuestas.MapGet("/buscar", (string? nombre, string? documento, string? parcela, string? page, string? pageSize, IServiceProvider services) =>
        {
            nombre = NullIfEmpty(nombre);
            if (!TryParseDocumento(documento, out long? numeroDocumento) || !TryNormalizeNumero(parcela, out string? numeroParcela)
                || (nombre is null && numeroDocumento is null && numeroParcela is null)
                || !TryParseInt(page, 1, out int currentPage) || !TryParseInt(pageSize, DefaultPageSize, out int currentPageSize)
                || currentPage < 1 || currentPageSize is < 1 or > MaxPageSize)
                return InvalidRequest<PropuestaV2BusquedaResponse>();

            try
            {
                DtoBusquedaPropuestasV2Listado resultado = services.GetRequiredService<PropuestasService>().BuscarPropuestasV2(new DtoBusquedaPropuestasV2Req
                {
                    Nombre = nombre,
                    Documento = numeroDocumento,
                    Parcela = numeroParcela,
                    Page = currentPage,
                    PageSize = currentPageSize
                });
                return Ok(new PropuestaV2BusquedaResponse(resultado.Items.Select(MapBusqueda).ToList(),
                    new PaginacionV2(currentPage, currentPageSize, resultado.TotalItems, resultado.TotalPages,
                        currentPage < resultado.TotalPages, currentPage > 1)));
            }
            catch (ArgumentOutOfRangeException) { return InvalidRequest<PropuestaV2BusquedaResponse>(); }
            catch { return BusinessFailure<PropuestaV2BusquedaResponse>(); }
        });

        propuestas.MapGet("/{propuesta}", (string propuesta, IServiceProvider services) =>
            Execute<PropuestaV2DetalleResponse>(propuesta, services, (numero, codigo) =>
            {
                PropuestasService propuestaService = services.GetRequiredService<PropuestasService>();
                DtoPropuestaDetalleResp? detalle = propuestaService.DetalleV2(codigo);
                if (detalle is null)
                    return NotFound<PropuestaV2DetalleResponse>();
                return Ok(MapDetalle(numero, detalle, propuestaService.EstadoDeuda(codigo).Valor));
            }));

        propuestas.MapGet("/{propuesta}/titulares", (string propuesta, IServiceProvider services) =>
            Execute<PropuestaV2TitularesResponse>(propuesta, services, (numero, codigo) =>
            {
                List<DtoClientesPropuestas> titulares = services.GetRequiredService<PropuestasService>().TitularesV2(codigo);
                return Ok(new PropuestaV2TitularesResponse(numero, titulares.Count, titulares.Select(MapTitular).ToList()));
            }));

        propuestas.MapGet("/{propuesta}/deuda", (string propuesta, IServiceProvider services) =>
            Execute<PropuestaV2DeudaResponse>(propuesta, services, (numero, codigo) =>
            {
                CuentasCorrientesService cuentasService = services.GetRequiredService<CuentasCorrientesService>();
                Resultados<DtoResumenDeudaPropuesta> resumen = cuentasService.ResumenDeuda(new DtoCuentasCorrientesResumenReq { CodPropuesta = codigo });
                if (resumen.HayError || resumen.Valor is null)
                    return BusinessFailure<PropuestaV2DeudaResponse>();
                Dictionary<(string Tipo, int Codigo), DtoEstadoDeudaCuenta> estados = cuentasService.EstadosDeuda(codigo).Valor?
                    .ToDictionary(x => (x.Tipo, x.Codigo)) ?? new();
                DtoEstadoDeuda? estado = services.GetRequiredService<PropuestasService>().EstadoDeuda(codigo).Valor;
                return Ok(new PropuestaV2DeudaResponse(numero, MapEstado(estado), MapTotales(resumen.Valor.Totales),
                    resumen.Valor.Cuentas.Select(x => MapCuenta(x, estados.GetValueOrDefault((x.Tipo, x.Codigo)))).ToList()));
            }));

        propuestas.MapGet("/{propuesta}/servicios", (string propuesta, IServiceProvider services) =>
            Execute<PropuestaV2ServiciosResponse>(propuesta, services, (numero, codigo) =>
            {
                DtoServiciosPropuestaV2 servicios = services.GetRequiredService<ServiciosModelosService>().ServiciosPropuestaV2(codigo);
                return Ok(new PropuestaV2ServiciosResponse(numero,
                    servicios.Habilitaciones.Select(MapHabilitacion).ToList(),
                    servicios.Cupos.Select(x => new CupoServicioV2(x.Producto, x.Servicio, x.Total, x.Utilizados, x.Disponibles)).ToList(),
                    servicios.Utilizaciones.Select(MapUtilizacion).ToList()));
            }));

        propuestas.MapGet("/{propuesta}/contratos", (string propuesta, IServiceProvider services) =>
            Execute<PropuestaV2ContratosResponse>(propuesta, services, (numero, codigo) =>
            {
                DtoPropuestaDetalleContratosResp datos = services.GetRequiredService<PropuestasService>().ContratosV2(codigo);
                Dictionary<int, DtoPlanesVentasPropuestas> planes = (datos.ListPlanesVentas ?? new()).ToDictionary(x => x.CodContrato);
                return Ok(new PropuestaV2ContratosResponse(numero,
                    (datos.ListContratos ?? new()).Select(x => MapContrato(x, x.CodPlanVenta is { } id && planes.TryGetValue(x.CodContrato, out DtoPlanesVentasPropuestas? plan) ? plan : null)).ToList(),
                    new FacturacionV2((datos.ListTitularesFacturasPagos ?? new()).Select(x => new TitularFacturacionV2(x.CodCliente, x.Nombre, x.Factura, x.Pago)).ToList())));
            }));

        propuestas.MapGet("/{propuesta}/historial-titulares", (string propuesta, IServiceProvider services) =>
            Execute<PropuestaV2HistorialTitularesResponse>(propuesta, services, (numero, codigo) => Ok(new PropuestaV2HistorialTitularesResponse(numero,
                services.GetRequiredService<PropuestasService>().HistorialTitularesV2(codigo).Select(x =>
                    new HistorialTitularV2(new ClienteReferenciaV2(x.CodCliente, x.Nombre), DateOnly.FromDateTime(x.FechaAlta), x.FechaBaja,
                        NullIfEmpty(x.UsuarioAlta), NullIfEmpty(x.UsuarioBaja))).ToList()))));

        propuestas.MapGet("/{propuesta}/comprobantes", (string propuesta, string? desde, string? hasta, string? tipo, string? estado, string? page, string? pageSize, IServiceProvider services) =>
        {
            if (!TryParseInt(page, 1, out int currentPage) || !TryParseInt(pageSize, DefaultPageSize, out int currentPageSize)
                || !TryParseDate(desde, out DateOnly? fechaDesde) || !TryParseDate(hasta, out DateOnly? fechaHasta)
                || currentPage < 1 || currentPageSize is < 1 or > MaxPageSize || (fechaDesde is not null && fechaHasta is not null && fechaDesde > fechaHasta))
                return InvalidRequest<PropuestaV2ComprobantesResponse>();
            return Execute<PropuestaV2ComprobantesResponse>(propuesta, services, (numero, codigo) =>
            {
                DtoComprobantesPropuestaV2Listado resultado = services.GetRequiredService<ComprobantesService>()
                    .ComprobantesPropuestaV2(codigo, fechaDesde, fechaHasta, tipo, estado, currentPage, currentPageSize);
                return Ok(new PropuestaV2ComprobantesResponse(numero, resultado.Items.Select(x => new ComprobanteV2(x.Fecha,
                    x.TipoComprobante, x.NroComprobante, new ClienteReferenciaV2(x.CodCliente, x.Cliente), x.Total, x.Pago,
                    x.EstadoComprobante, x.Anulado)).ToList(), new PaginacionV2(currentPage, currentPageSize, resultado.TotalItems,
                    resultado.TotalPages, currentPage < resultado.TotalPages, currentPage > 1)));
            });
        });

        return group;
    }

    private static IResult Execute<T>(string propuesta, IServiceProvider services, Func<int, int, IResult> action)
    {
        if (!int.TryParse(propuesta, out int numeroPropuesta) || numeroPropuesta <= 0)
            return InvalidRequest<T>();
        try
        {
            Resultados<DtoPropuestaResuelta> resolucion = services.GetRequiredService<PropuestasService>().ResolverPropuesta(numeroPropuesta);
            return resolucion.HayError || resolucion.Valor is null ? NotFound<T>() : action(resolucion.Valor.Propuesta, resolucion.Valor.CodPropuesta);
        }
        catch (ArgumentOutOfRangeException) { return InvalidRequest<T>(); }
        catch { return BusinessFailure<T>(); }
    }

    private static IResult Ok<T>(T data) => Results.Ok(ElPrado.McpApi.Contracts.ApiResponse<T>.Success(data));
    private static IResult InvalidRequest<T>() => Failure<T>("INVALID_REQUEST", "La solicitud no es válida.", StatusCodes.Status400BadRequest);
    private static IResult NotFound<T>() => Failure<T>("PROPOSAL_NOT_FOUND", "La propuesta solicitada no está disponible.", StatusCodes.Status404NotFound);
    private static IResult BusinessFailure<T>() => Failure<T>("BUSINESS_OPERATION_FAILED", "No se pudo completar la consulta solicitada.", StatusCodes.Status422UnprocessableEntity);
    private static IResult Failure<T>(string code, string message, int status) => Results.Json(ElPrado.McpApi.Contracts.ApiResponse<T>.Failure(code, message), statusCode: status);

    private static PropuestaV2DetalleResponse MapDetalle(int propuesta, DtoPropuestaDetalleResp source, DtoEstadoDeuda? estadoDeuda)
    {
        List<DtoInhumadosPropuestas> inhumados = source.ListInhumados ?? new();
        Dictionary<int, DtoInhumadosPropuestas> porDetalle = inhumados.ToDictionary(x => x.CodDetInhumado);
        return new PropuestaV2DetalleResponse(
            propuesta, source.CodPropuesta, NullIfEmpty(source.TipoPropuesta), source.Fecha, source.FechaBaja, NullIfEmpty(source.EstadoDeuda), MapEstado(estadoDeuda),
            source.CodParcela is null ? null : new ParcelaV2(source.CodParcela.Value, NullIfEmpty(source.Parcela), NullIfEmpty(source.Manzana), MapEstadoParcela(source.EstadoParcelaDetalle),
                source.ListZonasParcelas ?? new List<string>(), (source.ListLugares ?? new()).Select(x => new LugarParcelaV2(x.CodNivel, x.CodLugar,
                    new EstadoLugarV2(NullIfEmpty(x.Estado), x.DisponibleVenta, x.DisponibleInhumar),
                    x.CodDetInhumado is { } detalle && porDetalle.TryGetValue(detalle, out DtoInhumadosPropuestas? inhumado) ? new InhumadoReferenciaV2(inhumado.CodDetInhumado, inhumado.NombreInhumado) : null)).ToList()),
            inhumados.Select(x => new InhumadoV2(x.CodDetInhumado, x.NombreInhumado, new DocumentoV2(NullIfEmpty(x.TipoDocumento), x.NroDocumento?.ToString()), x.FechaNacimiento, x.FechaFallecimiento, x.FechaInhumacion, x.FechaExhumacion)).ToList(),
            (source.ListPropuestasAsociadas ?? new()).Select(x => new PropuestaAsociadaV2(x.Propuesta, NullIfEmpty(x.Parcela))).ToList(),
            source.MuestraMensajeAlerta && !string.IsNullOrWhiteSpace(source.MensajeAlerta) ? new[] { source.MensajeAlerta } : Array.Empty<string>());
    }

    private static TitularV2 MapTitular(DtoClientesPropuestas x) => new(x.CodCliente, x.Orden, x.Orden == 1, x.Nombre,
        new DocumentoV2(NullIfEmpty(x.TipoDocumento), x.NroDocumento.ToString()), new ContactoV2(NullIfEmpty(x.Telefono), NullIfEmpty(x.TelefonoMovil), NullIfEmpty(x.Email)),
        new DomicilioV2(NullIfEmpty(x.Direccion), NullIfEmpty(x.Localidad), NullIfEmpty(x.Provincia)), x.FechaNacimiento, x.FechaAlta);
    private static EstadoDeudaV2? MapEstado(DtoEstadoDeuda? x) => x is null ? null : new EstadoDeudaV2(NullIfEmpty(x.Nombre), x.Activa, x.AlDia ?? false, x.PermiteImputar);
    private static EstadoParcelaV2? MapEstadoParcela(DtoEstadoParcela? x) => x is null ? null : new EstadoParcelaV2(NullIfEmpty(x.Nombre), x.Permanente, x.DisponibleVenta, x.DisponibleInhumar, x.Inhabilitada, x.ConInhumado);
    private static TotalesDeudaV2 MapTotales(DtoTotalesDeuda x) => new(x.Vencido, x.Intereses, x.AVencer, x.DescuentoVencido, x.DescuentoAVencer, x.Deuda, x.Total);
    private static CuentaCorrienteV2 MapCuenta(DtoCuentasCorrientesResumen x, DtoEstadoDeudaCuenta? estado) => new(x.Codigo, x.Tipo, NullIfEmpty(x.Categoria), MapEstado(estado), new ClienteReferenciaV2(x.CodCliente, x.Cliente), (decimal)x.Importe,
        new PeriodoCuentaV2(x.FechaInicio, x.PrimerCuota, x.CuotaDesde, x.CuotaHasta), new TotalesDeudaV2((decimal)x.ImporteVencido, (decimal)x.Interes, (decimal)x.ImporteAVencer, (decimal)x.DescuentoVencido, (decimal)x.DescuentoAVencer, (decimal)x.Deuda, (decimal)x.Total),
        new CobranzaV2(NullIfEmpty(x.Cobrador), NullIfEmpty(x.ZonaCobranza), NullIfEmpty(x.Comercializadora)));
    private static HabilitacionServicioV2 MapHabilitacion(DtoHabilitacionServicioV2 x) => new(x.CodCliente is null ? null : new ClienteReferenciaV2(x.CodCliente.Value, x.Cliente), x.Producto, x.Servicio, x.Habilitado,
        x.MotivoCodigo is null ? null : new MotivoHabilitacionV2(x.MotivoCodigo, x.MotivoDescripcion, x.MotivoHasta));
    private static UtilizacionServicioV2 MapUtilizacion(DtoUtilizacionServicioV2 x) => new(x.Fecha, x.Producto, x.Servicio, new BeneficiarioServicioV2(x.Beneficiario, new DocumentoV2(x.TipoDocumento, x.NroDocumento?.ToString())),
        string.IsNullOrWhiteSpace(x.NroComprobante) ? null : new ComprobanteReferenciaV2(x.NroComprobante), x.PropuestaOrigen, x.PropuestaAplicacion);
    private static ContratoV2 MapContrato(DtoContratosPropuestas x, DtoPlanesVentasPropuestas? plan) => new(x.CodContrato, x.Fecha, x.TipoContrato, x.Modelo, x.Estado, (decimal)x.Total, new VendedorV2(NullIfEmpty(x.Vendedor)),
        plan is null ? null : new PlanVentaV2(plan.CodPlanVenta, plan.PlanVenta, plan.Concepto));
    private static PropuestaBusquedaV2 MapBusqueda(DtoBusquedaPropuestaV2 x) => new(x.Propuesta, NullIfEmpty(x.Tipo), NullIfEmpty(x.Estado),
        string.IsNullOrWhiteSpace(x.Parcela) ? null : new ParcelaBusquedaV2(x.Parcela), x.Titulares.Select(t => new TitularBusquedaV2(NullIfEmpty(t.Nombre),
            t.NroDocumento is null ? null : new DocumentoV2(NullIfEmpty(t.TipoDocumento), t.NroDocumento.Value.ToString()))).ToList());
    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
    private static bool TryParseDate(string? value, out DateOnly? date)
    {
        date = null;
        if (string.IsNullOrWhiteSpace(value))
            return true;
        if (!DateOnly.TryParse(value, out DateOnly parsed))
            return false;
        date = parsed;
        return true;
    }
    private static bool TryParseInt(string? value, int defaultValue, out int number) =>
        string.IsNullOrWhiteSpace(value) ? (number = defaultValue) > 0 : int.TryParse(value, out number);
    private static bool TryParseDocumento(string? value, out long? documento)
    {
        documento = null;
        if (string.IsNullOrWhiteSpace(value))
            return true;
        string normalizado = new(value.Where(char.IsDigit).ToArray());
        if (normalizado.Length == 0 || !long.TryParse(normalizado, out long numero) || numero <= 0)
            return false;
        documento = numero;
        return true;
    }
    private static bool TryNormalizeNumero(string? value, out string? numero)
    {
        numero = null;
        if (string.IsNullOrWhiteSpace(value))
            return true;
        string normalizado = new(value.Where(char.IsDigit).ToArray());
        if (normalizado.Length == 0)
            return false;
        numero = normalizado;
        return true;
    }
}
