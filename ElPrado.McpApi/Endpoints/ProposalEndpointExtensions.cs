using ElPrado.Core;
using ElPrado.Core.Enums;
using ElPrado.Dto.Dtos;
using ElPrado.McpApi.Contracts;
using ElPrado.Services.Services;

namespace ElPrado.McpApi.Endpoints;

public static class ProposalEndpointExtensions
{
    public static RouteGroupBuilder MapProposalOperations(this RouteGroupBuilder group)
    {
        RouteGroupBuilder proposals = group.MapGroup("/propuestas");

        proposals.MapPost("/detalle", (McpProposalRequest request, IServiceProvider services) =>
            ExecuteProposal(request, () => services.GetRequiredService<PropuestasService>().Detalle(ToDomain(request)), MapDetail));

        proposals.MapPost("/titulares", (McpProposalRequest request, IServiceProvider services) =>
            ExecuteProposal(request, () => services.GetRequiredService<PropuestasService>().Titulares(ToDomain(request)), holders => holders.Select(MapHolder).ToList()));

        proposals.MapPost("/cuenta-corriente", (McpCurrentAccountRequest request, IServiceProvider services) =>
        {
            if (request.CodPropuesta <= 0) return InvalidRequest<McpCurrentAccountResponse>();
            return Execute(() => services.GetRequiredService<CuentasCorrientesService>().ResumenCuentas(new DtoCuentasCorrientesResumenReq
            {
                CodPropuesta = request.CodPropuesta,
                MostrarBaja = request.MostrarBaja,
                MostrarInactiva = request.MostrarInactiva,
                FechaInteres = request.FechaInteres,
                FechaHasta = request.FechaHasta
            }), accounts => accounts.Select(MapAccount).ToList());
        });

        proposals.MapPost("/servicios", (McpServicesRequest request, IServiceProvider services) =>
        {
            if (request.CodPropuesta <= 0) return InvalidRequest<McpProposalServicesResponse>();
            try
            {
                return Results.Ok(ElPrado.McpApi.Contracts.ApiResponse<McpProposalServicesResponse>.Success(MapServices(services.GetRequiredService<ServiciosModelosService>().ServiciosPropuesta(request.CodPropuesta))));
            }
            catch
            {
                return Failure<McpProposalServicesResponse>("BUSINESS_OPERATION_FAILED", "No se pudo completar la consulta solicitada.", StatusCodes.Status422UnprocessableEntity);
            }
        });

        proposals.MapPost("/comprobantes", async (McpComprobantesRequest request, IServiceProvider services) =>
        {
            if (request.Propuesta <= 0 || request.Pagina <= 0 || request.FilasPagina is < 1 or > 100)
                return InvalidRequest<McpComprobantesResponse>();

            try
            {
                ApiResponseListado<IEnumerable<dynamic>> result = await services.GetRequiredService<ComprobantesService>().ListadoComprobantesPropuestaAsync(new DtoOpcionesListados
                {
                    MostrarFiltros = false,
                    Pagina = request.Pagina,
                    FilasPagina = request.FilasPagina,
                    SinPaginado = false,
                    ListFiltros = new List<DtoCamposFiltroListado>
                    {
                        new()
                        {
                            Campo = "codPropuesta",
                            TipoComparacion = TipoComparacion.Igual,
                            Valor = request.Propuesta.ToString()
                        }
                    }
                });

                if (!result.Success)
                    return Failure<McpComprobantesResponse>("BUSINESS_OPERATION_FAILED", "No se pudieron consultar los comprobantes.", StatusCodes.Status422UnprocessableEntity);

                IReadOnlyList<McpProposalComprobanteResponse> items = (result.Data ?? Enumerable.Empty<dynamic>())
                    .Cast<DtoComprobantesPropuestaList>()
                    .Select(MapComprobante)
                    .ToList();
                return Results.Ok(ElPrado.McpApi.Contracts.ApiResponse<McpComprobantesResponse>.Success(new McpComprobantesResponse(items, result.CantidadPaginas, result.CantidadRegistros)));
            }
            catch
            {
                return Failure<McpComprobantesResponse>("BUSINESS_OPERATION_FAILED", "No se pudo completar la consulta solicitada.", StatusCodes.Status422UnprocessableEntity);
            }
        });

        proposals.MapPost("/contratos", (McpProposalRequest request, IServiceProvider services) =>
            ExecuteProposal(request, () => services.GetRequiredService<PropuestasService>().DetalleContratos(ToDomain(request)), MapContracts));

        proposals.MapPost("/historial-titulares", (McpProposalRequest request, IServiceProvider services) =>
            ExecuteProposal(request, () => services.GetRequiredService<PropuestasService>().DetalleHistorialTitulares(ToDomain(request)), history => history.Select(MapHistory).ToList()));

        return group;
    }

    private static IResult ExecuteProposal<TSource, TResult>(McpProposalRequest request, Func<Resultados<TSource>> action, Func<TSource, TResult> map)
    {
        if (request.Propuesta <= 0) return InvalidRequest<TResult>();
        return Execute(action, map);
    }

    private static IResult Execute<TSource, TResult>(Func<Resultados<TSource>> action, Func<TSource, TResult> map)
    {
        try
        {
            return Execute(action(), map);
        }
        catch (ArgumentOutOfRangeException)
        {
            return InvalidRequest<TResult>();
        }
        catch
        {
            return Failure<TResult>("BUSINESS_OPERATION_FAILED", "No se pudo completar la consulta solicitada.", StatusCodes.Status422UnprocessableEntity);
        }
    }

    private static IResult Execute<TSource, TResult>(Resultados<TSource> result, Func<TSource, TResult> map)
    {
        if (result.HayError || result.Valor is null)
        {
            bool unavailable = result.Errores.Any(error => error.Contains("no existe", StringComparison.OrdinalIgnoreCase)
                || error.Contains("no est", StringComparison.OrdinalIgnoreCase));
            return Failure<TResult>(
                unavailable ? "PROPOSAL_NOT_FOUND" : "INVALID_REQUEST",
                unavailable ? "La propuesta solicitada no está disponible." : "La solicitud no es válida.",
                unavailable ? StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest);
        }

        return Results.Ok(ElPrado.McpApi.Contracts.ApiResponse<TResult>.Success(map(result.Valor)));
    }

    private static IResult InvalidRequest<T>() =>
        Failure<T>("INVALID_REQUEST", "La solicitud no es válida.", StatusCodes.Status400BadRequest);

    private static IResult Failure<T>(string code, string message, int statusCode) =>
        Results.Json(ElPrado.McpApi.Contracts.ApiResponse<T>.Failure(code, message), statusCode: statusCode);

    private static DtoPropuestaDetalleReq ToDomain(McpProposalRequest request) => new()
    {
        Propuesta = request.Propuesta,
        IncluirBaja = request.IncluirBaja
    };

    private static McpProposalDetailResponse MapDetail(DtoPropuestaDetalleResp source) => new(
        source.CodPropuesta, source.Propuesta, source.TipoPropuesta, source.Fecha, source.FechaBaja,
        source.EstadoDeuda, source.MuestraMensajeAlerta, source.MensajeAlerta, source.CodParcela,
        source.Parcela, source.Manzana, source.EstadoParcela,
        (source.ListPropuestasAsociadas ?? new()).Select(x => new McpAssociatedProposalResponse(x.CodPropuesta, x.Propuesta, x.Parcela, x.PrimerTitular)).ToList(),
        source.ListZonasParcelas ?? new(),
        (source.ListDetalleLugares ?? new()).Select(x => new McpProposalPlaceResponse(x.Nivel, x.Lugares, x.NombreInhumado1, x.Estado1, x.Color1, x.NombreInhumado2, x.Estado2, x.Color2, x.NombreInhumado3, x.Estado3, x.Color3, x.NombreInhumado4, x.Estado4, x.Color4, x.NombreInhumado5, x.Estado5, x.Color5, x.NombreInhumado6, x.Estado6, x.Color6)).ToList(),
        (source.ListInhumados ?? new()).Select(x => new McpProposalDeceasedResponse(x.CodDetInhumado, x.NombreInhumado, x.TipoDocumento, x.NroDocumento, x.FechaNacimiento, x.FechaFallecimiento, x.FechaInhumacion, x.FechaExhumacion, x.NombreBIM, x.NroDeclaracionJurada)).ToList());

    private static McpProposalHolderResponse MapHolder(DtoClientesPropuestas source) => new(
        source.CodCliente, source.Orden, source.Nombre, source.TipoDocumento, source.NroDocumento,
        source.Telefono, source.TelefonoMovil, source.Email, source.Direccion, source.Localidad,
        source.Provincia, source.FechaNacimiento, source.FechaAlta, source.Web);

    private static McpCurrentAccountResponse MapAccount(DtoCuentasCorrientesResumen source) => new(
        source.Tipo, source.EsRefinanciacion, source.EsDocumentado, source.Categoria, source.Estado,
        source.Importe, source.PrecioDolar, source.FechaInicio, source.PrimerCuota, source.FechaBaja,
        source.Cobrador, source.Cliente, source.CodCliente, source.ZonaCobranza, source.CobradorZona,
        source.Codigo, source.CodMedioCobro, source.Comercializadora, source.Cuotas, source.CuotaDesde,
        source.CuotaHasta, source.ImporteVencido, source.Interes, source.ImporteAVencer,
        source.DescuentoVencido, source.DescuentoAVencer, source.Deuda, source.Total, source.PorcCancelado,
        source.CodCatRecargo, source.EsIndependiente, source.EsPrecioDiferencial);

    private static McpProposalServicesResponse MapServices(DtoServiciosPropuesta source) => new(
        (source.ListServiciosHabilitados ?? new()).Select(MapEnabledService).ToList(),
        (source.ListServiciosUtilizados ?? new()).Select(x => new McpUsedServiceResponse(x.Servicio, x.ServiciosRealizados, x.ServiciosPendientes)).ToList(),
        (source.ListServiciosBeneficiarios ?? new()).Select(x => new McpServiceBeneficiaryResponse(x.Fecha, x.NroComprobante, x.TipoServicio, x.CodTalonario, x.Nombre, x.NroDocumento, x.Producto, x.UsadoDe, x.UsadoEn)).ToList());

    private static McpEnabledServiceResponse MapEnabledService(DtoServiciosHabilitados source) => new(
        source.PlanBeneficio, source.Cliente,
        new[]
        {
            new McpServiceMessageResponse(source.TituloServicio01, source.MensajeServicio01), new McpServiceMessageResponse(source.TituloServicio02, source.MensajeServicio02), new McpServiceMessageResponse(source.TituloServicio03, source.MensajeServicio03), new McpServiceMessageResponse(source.TituloServicio04, source.MensajeServicio04), new McpServiceMessageResponse(source.TituloServicio05, source.MensajeServicio05), new McpServiceMessageResponse(source.TituloServicio06, source.MensajeServicio06), new McpServiceMessageResponse(source.TituloServicio07, source.MensajeServicio07), new McpServiceMessageResponse(source.TituloServicio08, source.MensajeServicio08), new McpServiceMessageResponse(source.TituloServicio09, source.MensajeServicio09), new McpServiceMessageResponse(source.TituloServicio10, source.MensajeServicio10), new McpServiceMessageResponse(source.TituloServicio11, source.MensajeServicio11), new McpServiceMessageResponse(source.TituloServicio12, source.MensajeServicio12), new McpServiceMessageResponse(source.TituloServicio13, source.MensajeServicio13), new McpServiceMessageResponse(source.TituloServicio14, source.MensajeServicio14), new McpServiceMessageResponse(source.TituloServicio15, source.MensajeServicio15)
        }, source.CodCliente, source.CantServicios);

    private static McpProposalComprobanteResponse MapComprobante(DtoComprobantesPropuestaList source) => new(
        source.Fecha, source.TipoComprobante, source.Talonario, source.NroComprobante, source.Cliente,
        source.Total, source.Estado, source.Concepto, source.Anulado, source.Pago, source.Usuario,
        source.NroComprobanteImputacion, source.ClienteBeneficiado, source.CodClienteBeneficiado,
        source.CodTalonario, source.CodTalonarioAsociado1, source.CodMovimientoFondo);

    private static McpProposalContractsResponse MapContracts(DtoPropuestaDetalleContratosResp source) => new(
        (source.ListContratos ?? new()).Select(x => new McpProposalContractResponse(x.CodContrato, x.CodPlanVenta, x.Fecha, x.FechaBaja, x.FechaCaducidad, x.TipoContrato, x.Modelo, x.Vendedor, x.Total)).ToList(),
        (source.ListPlanesVentas ?? new()).Select(x => new McpSalesPlanResponse(x.PlanVenta, x.Concepto, x.Total, x.VigenciaDesde, x.VigenciaHasta)).ToList(),
        (source.ListTitulares ?? new()).Select(MapHolder).ToList(),
        (source.ListTitularesFacturasPagos ?? new()).Select(x => new McpInvoicePaymentHolderResponse(x.CodCliente, x.Factura, x.Pago, x.Nombre, x.TipoDocumento, x.NroDocumento, x.Telefono, x.TelefonoMovil)).ToList());

    private static McpHolderHistoryResponse MapHistory(DtoClientesPropuestasHistorial source) => new(
        source.Nombre, source.FechaAlta, source.AutorizaAlta, source.UsuarioAlta, source.FechaBaja,
        source.AutorizaBaja, source.UsuarioBaja);
}
