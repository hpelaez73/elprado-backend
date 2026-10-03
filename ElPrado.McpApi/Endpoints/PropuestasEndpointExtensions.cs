using ElPrado.Core;
using ElPrado.Dto.Dtos;
using ElPrado.McpApi.Contracts;
using ElPrado.McpApi.Observability;
using ElPrado.Services.Services;
using Microsoft.Extensions.Logging;

namespace ElPrado.McpApi.Endpoints;

/// <summary>HTTP read adapter for the public, agent-oriented proposal API.</summary>
public static class PropuestasEndpointExtensions
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    public static RouteGroupBuilder MapPropuestasOperations(this RouteGroupBuilder group)
    {
        RouteGroupBuilder propuestas = group.MapGroup("/propuestas").WithTags("Propuestas");

        propuestas.MapGet("/buscar", (string? nombre, string? documento, string? parcela, string? page, string? pageSize, IServiceProvider services, HttpContext context, ILoggerFactory loggerFactory) =>
        {
            nombre = NullIfEmpty(nombre);
            if (!TryParseDocumento(documento, out long? numeroDocumento) || !TryNormalizeNumero(parcela, out string? numeroParcela)
                || (nombre is null && numeroDocumento is null && numeroParcela is null)
                || !TryParseInt(page, 1, out int currentPage) || !TryParseInt(pageSize, DefaultPageSize, out int currentPageSize)
                || currentPage < 1 || currentPageSize is < 1 or > MaxPageSize)
                return InvalidRequest<PropuestaBusquedaResponse>();

            try
            {
                DtoBusquedaPropuestasListado resultado = services.GetRequiredService<PropuestasService>().BuscarPropuestas(new DtoBusquedaPropuestasReq
                {
                    Nombre = nombre,
                    Documento = numeroDocumento,
                    Parcela = numeroParcela,
                    Page = currentPage,
                    PageSize = currentPageSize
                });
                return Ok(new PropuestaBusquedaResponse(resultado.Items.Select(MapBusqueda).ToList(),
                    new Paginacion(currentPage, currentPageSize, resultado.TotalItems, resultado.TotalPages,
                        currentPage < resultado.TotalPages, currentPage > 1)));
            }
            catch (ArgumentOutOfRangeException exception)
            {
                McpExceptionLog.Error(loggerFactory.CreateLogger(nameof(PropuestasEndpointExtensions)), context, exception, StatusCodes.Status400BadRequest);
                return InvalidRequest<PropuestaBusquedaResponse>();
            }
            catch (Exception exception)
            {
                McpExceptionLog.Error(loggerFactory.CreateLogger(nameof(PropuestasEndpointExtensions)), context, exception, StatusCodes.Status422UnprocessableEntity);
                return BusinessFailure<PropuestaBusquedaResponse>();
            }
        });

        propuestas.MapGet("/{propuesta}", (string propuesta, IServiceProvider services, HttpContext context, ILoggerFactory loggerFactory) =>
            Execute<PropuestaDetalleResponse>(propuesta, services, context, loggerFactory.CreateLogger(nameof(PropuestasEndpointExtensions)), (numero, codigo) =>
            {
                PropuestasService propuestaService = services.GetRequiredService<PropuestasService>();
                DtoPropuestaDetalleResp? detalle = propuestaService.Detalle(codigo);
                if (detalle is null)
                    return NotFound<PropuestaDetalleResponse>();
                return Ok(MapDetalle(numero, detalle, propuestaService.EstadoDeuda(codigo).Valor));
            }));

        propuestas.MapGet("/{propuesta}/titulares", (string propuesta, IServiceProvider services, HttpContext context, ILoggerFactory loggerFactory) =>
            Execute<PropuestaTitularesResponse>(propuesta, services, context, loggerFactory.CreateLogger(nameof(PropuestasEndpointExtensions)), (numero, codigo) =>
            {
                List<DtoClientesPropuestas> titulares = services.GetRequiredService<PropuestasService>().Titulares(codigo);
                return Ok(new PropuestaTitularesResponse(numero, titulares.Count, titulares.Select(MapTitular).ToList()));
            }));

        propuestas.MapGet("/{propuesta}/deuda", (string propuesta, IServiceProvider services, HttpContext context, ILoggerFactory loggerFactory) =>
            Execute<PropuestaDeudaResponse>(propuesta, services, context, loggerFactory.CreateLogger(nameof(PropuestasEndpointExtensions)), (numero, codigo) =>
            {
                CuentasCorrientesService cuentasService = services.GetRequiredService<CuentasCorrientesService>();
                Resultados<DtoResumenDeudaPropuesta> resumen = cuentasService.ResumenDeuda(new DtoCuentasCorrientesResumenReq { CodPropuesta = codigo });
                if (resumen.HayError || resumen.Valor is null)
                    return BusinessFailure<PropuestaDeudaResponse>();
                Dictionary<(string Tipo, int Codigo), DtoEstadoDeudaCuenta> estados = cuentasService.EstadosDeuda(codigo).Valor?
                    .ToDictionary(x => (x.Tipo, x.Codigo)) ?? new();
                DtoEstadoDeuda? estado = services.GetRequiredService<PropuestasService>().EstadoDeuda(codigo).Valor;
                return Ok(new PropuestaDeudaResponse(numero, MapEstado(estado), MapTotales(resumen.Valor.Totales),
                    resumen.Valor.Cuentas.Select(x => MapCuenta(x, estados.GetValueOrDefault((x.Tipo, x.Codigo)))).ToList()));
            }));

        propuestas.MapGet("/{propuesta}/servicios", (string propuesta, IServiceProvider services, HttpContext context, ILoggerFactory loggerFactory) =>
            Execute<PropuestaServiciosResponse>(propuesta, services, context, loggerFactory.CreateLogger(nameof(PropuestasEndpointExtensions)), (numero, codigo) =>
            {
                DtoServiciosPropuestaMcp servicios = services.GetRequiredService<ServiciosModelosService>().ServiciosPropuestaMcp(codigo);
                return Ok(new PropuestaServiciosResponse(numero,
                    servicios.Habilitaciones.Select(MapHabilitacion).ToList(),
                    servicios.Cupos.Select(x => new CupoServicio(x.Producto, x.Servicio, x.Total, x.Utilizados, x.Disponibles)).ToList(),
                    servicios.Utilizaciones.Select(MapUtilizacion).ToList()));
            }));

        propuestas.MapGet("/{propuesta}/contratos", (string propuesta, IServiceProvider services, HttpContext context, ILoggerFactory loggerFactory) =>
            Execute<PropuestaContratosResponse>(propuesta, services, context, loggerFactory.CreateLogger(nameof(PropuestasEndpointExtensions)), (numero, codigo) =>
            {
                DtoPropuestaDetalleContratosResp datos = services.GetRequiredService<PropuestasService>().Contratos(codigo);
                Dictionary<int, DtoPlanesVentasPropuestas> planes = (datos.ListPlanesVentas ?? new()).ToDictionary(x => x.CodContrato);
                return Ok(new PropuestaContratosResponse(numero,
                    (datos.ListContratos ?? new()).Select(x => MapContrato(x, x.CodPlanVenta is { } id && planes.TryGetValue(x.CodContrato, out DtoPlanesVentasPropuestas? plan) ? plan : null)).ToList(),
                    new Facturacion((datos.ListTitularesFacturasPagos ?? new()).Select(x => new TitularFacturacion(x.CodCliente, x.Nombre, x.Factura, x.Pago)).ToList())));
            }));

        propuestas.MapGet("/{propuesta}/historial-titulares", (string propuesta, IServiceProvider services, HttpContext context, ILoggerFactory loggerFactory) =>
            Execute<PropuestaHistorialTitularesResponse>(propuesta, services, context, loggerFactory.CreateLogger(nameof(PropuestasEndpointExtensions)), (numero, codigo) => Ok(new PropuestaHistorialTitularesResponse(numero,
                services.GetRequiredService<PropuestasService>().HistorialTitulares(codigo).Select(x =>
                    new HistorialTitular(new ClienteReferencia(x.CodCliente, x.Nombre), DateOnly.FromDateTime(x.FechaAlta), x.FechaBaja,
                        NullIfEmpty(x.UsuarioAlta), NullIfEmpty(x.UsuarioBaja))).ToList()))));

        propuestas.MapGet("/{propuesta}/comprobantes", (string propuesta, string? desde, string? hasta, string? tipo, string? estado, string? page, string? pageSize, IServiceProvider services, HttpContext context, ILoggerFactory loggerFactory) =>
        {
            if (!TryParseInt(page, 1, out int currentPage) || !TryParseInt(pageSize, DefaultPageSize, out int currentPageSize)
                || !TryParseDate(desde, out DateOnly? fechaDesde) || !TryParseDate(hasta, out DateOnly? fechaHasta)
                || currentPage < 1 || currentPageSize is < 1 or > MaxPageSize || (fechaDesde is not null && fechaHasta is not null && fechaDesde > fechaHasta))
                return InvalidRequest<PropuestaComprobantesResponse>();
            return Execute<PropuestaComprobantesResponse>(propuesta, services, context, loggerFactory.CreateLogger(nameof(PropuestasEndpointExtensions)), (numero, codigo) =>
            {
                DtoComprobantesPropuestaListado resultado = services.GetRequiredService<ComprobantesService>()
                    .ComprobantesPropuesta(codigo, fechaDesde, fechaHasta, tipo, estado, currentPage, currentPageSize);
                return Ok(new PropuestaComprobantesResponse(numero, resultado.Items.Select(x => new Comprobante(x.Fecha,
                    x.TipoComprobante, x.NroComprobante, new ClienteReferencia(x.CodCliente, x.Cliente), x.Total, x.Pago,
                    x.EstadoComprobante, x.Anulado)).ToList(), new Paginacion(currentPage, currentPageSize, resultado.TotalItems,
                    resultado.TotalPages, currentPage < resultado.TotalPages, currentPage > 1)));
            });
        });

        return group;
    }

    private static IResult Execute<T>(string propuesta, IServiceProvider services, HttpContext context, ILogger logger, Func<int, int, IResult> action)
    {
        if (!int.TryParse(propuesta, out int numeroPropuesta) || numeroPropuesta <= 0)
            return InvalidRequest<T>();
        try
        {
            Resultados<DtoPropuestaResuelta> resolucion = services.GetRequiredService<PropuestasService>().ResolverPropuesta(numeroPropuesta);
            return resolucion.HayError || resolucion.Valor is null ? NotFound<T>() : action(resolucion.Valor.Propuesta, resolucion.Valor.CodPropuesta);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            McpExceptionLog.Error(logger, context, exception, StatusCodes.Status400BadRequest);
            return InvalidRequest<T>();
        }
        catch (Exception exception)
        {
            McpExceptionLog.Error(logger, context, exception, StatusCodes.Status422UnprocessableEntity);
            return BusinessFailure<T>();
        }
    }

    private static IResult Ok<T>(T data) => Results.Ok(ElPrado.McpApi.Contracts.ApiResponse<T>.Success(data));
    private static IResult InvalidRequest<T>() => Failure<T>("INVALID_REQUEST", "La solicitud no es válida.", StatusCodes.Status400BadRequest);
    private static IResult NotFound<T>() => Failure<T>("PROPOSAL_NOT_FOUND", "La propuesta solicitada no está disponible.", StatusCodes.Status404NotFound);
    private static IResult BusinessFailure<T>() => Failure<T>("BUSINESS_OPERATION_FAILED", "No se pudo completar la consulta solicitada.", StatusCodes.Status422UnprocessableEntity);
    private static IResult Failure<T>(string code, string message, int status) => Results.Json(ElPrado.McpApi.Contracts.ApiResponse<T>.Failure(code, message), statusCode: status);

    private static PropuestaDetalleResponse MapDetalle(int propuesta, DtoPropuestaDetalleResp source, DtoEstadoDeuda? estadoDeuda)
    {
        List<DtoInhumadosPropuestas> inhumados = source.ListInhumados ?? new();
        Dictionary<int, DtoInhumadosPropuestas> porDetalle = inhumados.ToDictionary(x => x.CodDetInhumado);
        return new PropuestaDetalleResponse(
            propuesta, source.CodPropuesta, NullIfEmpty(source.TipoPropuesta), source.Fecha, source.FechaBaja, NullIfEmpty(source.EstadoDeuda), MapEstado(estadoDeuda),
            source.CodParcela is null ? null : new Parcela(source.CodParcela.Value, NullIfEmpty(source.Parcela), NullIfEmpty(source.Manzana), MapEstadoParcela(source.EstadoParcelaDetalle),
                source.ListZonasParcelas ?? new List<string>(), (source.ListLugares ?? new()).Select(x => new LugarParcela(x.CodNivel, x.CodLugar,
                    new EstadoLugar(NullIfEmpty(x.Estado), x.DisponibleVenta, x.DisponibleInhumar),
                    x.CodDetInhumado is { } detalle && porDetalle.TryGetValue(detalle, out DtoInhumadosPropuestas? inhumado) ? new InhumadoReferencia(inhumado.CodDetInhumado, inhumado.NombreInhumado) : null)).ToList()),
            inhumados.Select(x => new Inhumado(x.CodDetInhumado, x.NombreInhumado, new Documento(NullIfEmpty(x.TipoDocumento), x.NroDocumento?.ToString()), x.FechaNacimiento, x.FechaFallecimiento, x.FechaInhumacion, x.FechaExhumacion)).ToList(),
            (source.ListPropuestasAsociadas ?? new()).Select(x => new PropuestaAsociada(x.Propuesta, NullIfEmpty(x.Parcela))).ToList(),
            source.MuestraMensajeAlerta && !string.IsNullOrWhiteSpace(source.MensajeAlerta) ? new[] { source.MensajeAlerta } : Array.Empty<string>());
    }

    private static Titular MapTitular(DtoClientesPropuestas x) => new(x.CodCliente, x.Orden, x.Orden == 1, x.Nombre,
        new Documento(NullIfEmpty(x.TipoDocumento), x.NroDocumento.ToString()), new Contacto(NullIfEmpty(x.Telefono), NullIfEmpty(x.TelefonoMovil), NullIfEmpty(x.Email)),
        new Domicilio(NullIfEmpty(x.Direccion), NullIfEmpty(x.Localidad), NullIfEmpty(x.Provincia)), x.FechaNacimiento, x.FechaAlta);
    private static EstadoDeuda? MapEstado(DtoEstadoDeuda? x) => x is null ? null : new EstadoDeuda(NullIfEmpty(x.Nombre), x.Activa, x.AlDia ?? false, x.PermiteImputar);
    private static EstadoParcela? MapEstadoParcela(DtoEstadoParcela? x) => x is null ? null : new EstadoParcela(NullIfEmpty(x.Nombre), x.Permanente, x.DisponibleVenta, x.DisponibleInhumar, x.Inhabilitada, x.ConInhumado);
    private static TotalesDeuda MapTotales(DtoTotalesDeuda x) => new(x.Vencido, x.Intereses, x.AVencer, x.DescuentoVencido, x.DescuentoAVencer, x.Deuda, x.Total);
    private static CuentaCorriente MapCuenta(DtoCuentasCorrientesResumen x, DtoEstadoDeudaCuenta? estado) => new(x.Codigo, x.Tipo, NullIfEmpty(x.Categoria), MapEstado(estado), new ClienteReferencia(x.CodCliente, x.Cliente), (decimal)x.Importe,
        new PeriodoCuenta(x.FechaInicio, x.PrimerCuota, x.CuotaDesde, x.CuotaHasta), new TotalesDeuda((decimal)x.ImporteVencido, (decimal)x.Interes, (decimal)x.ImporteAVencer, (decimal)x.DescuentoVencido, (decimal)x.DescuentoAVencer, (decimal)x.Deuda, (decimal)x.Total),
        new Cobranza(NullIfEmpty(x.Cobrador), NullIfEmpty(x.ZonaCobranza), NullIfEmpty(x.Comercializadora)));
    private static HabilitacionServicio MapHabilitacion(DtoHabilitacionServicioMcp x) => new(x.CodCliente is null ? null : new ClienteReferencia(x.CodCliente.Value, x.Cliente), x.Producto, x.Servicio, x.Habilitado,
        x.MotivoCodigo is null ? null : new MotivoHabilitacion(x.MotivoCodigo, x.MotivoDescripcion, x.MotivoHasta));
    private static UtilizacionServicio MapUtilizacion(DtoUtilizacionServicioMcp x) => new(x.Fecha, x.Producto, x.Servicio, new BeneficiarioServicio(x.Beneficiario, new Documento(x.TipoDocumento, x.NroDocumento?.ToString())),
        string.IsNullOrWhiteSpace(x.NroComprobante) ? null : new ComprobanteReferencia(x.NroComprobante), x.PropuestaOrigen, x.PropuestaAplicacion);
    private static Contrato MapContrato(DtoContratosPropuestas x, DtoPlanesVentasPropuestas? plan) => new(x.CodContrato, x.Fecha, x.TipoContrato, x.Modelo, x.Estado, (decimal)x.Total, new Vendedor(NullIfEmpty(x.Vendedor)),
        plan is null ? null : new PlanVenta(plan.CodPlanVenta, plan.PlanVenta, plan.Concepto));
    private static PropuestaBusqueda MapBusqueda(DtoBusquedaPropuesta x) => new(x.Propuesta, NullIfEmpty(x.Tipo), NullIfEmpty(x.Estado),
        string.IsNullOrWhiteSpace(x.Parcela) ? null : new ParcelaBusqueda(x.Parcela), x.Titulares.Select(t => new TitularBusqueda(NullIfEmpty(t.Nombre),
            t.NroDocumento is null ? null : new Documento(NullIfEmpty(t.TipoDocumento), t.NroDocumento.Value.ToString()))).ToList());
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
