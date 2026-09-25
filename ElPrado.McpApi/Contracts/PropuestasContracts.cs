namespace ElPrado.McpApi.Contracts;

// These contracts intentionally belong to the MCP facade. They keep the
// transport boundary independent from the WebApi request/response envelopes.
public sealed record McpPropuestasRequest(int Propuesta, bool IncluirBaja = false);

public sealed record McpCurrentAccountRequest(
    int CodPropuesta,
    bool MostrarBaja = false,
    bool MostrarInactiva = false,
    DateOnly? FechaInteres = null,
    DateOnly? FechaHasta = null);

public sealed record McpServicesRequest(int CodPropuesta);

public sealed record McpComprobantesRequest(int Propuesta, int Pagina = 1, int FilasPagina = 20);

public sealed record McpPropuestasDetailResponse(
    int CodPropuesta,
    int Propuesta,
    string TipoPropuesta,
    DateOnly Fecha,
    DateOnly? FechaBaja,
    string EstadoDeuda,
    bool MuestraMensajeAlerta,
    string MensajeAlerta,
    int? CodParcela,
    string Parcela,
    string Manzana,
    string EstadoParcela,
    IReadOnlyList<McpAssociatedPropuestasResponse> PropuestasAsociadas,
    IReadOnlyList<string> ZonasParcela,
    IReadOnlyList<McpPropuestasPlaceResponse> Lugares,
    IReadOnlyList<McpPropuestasDeceasedResponse> Inhumados);

public sealed record McpAssociatedPropuestasResponse(int CodPropuesta, int Propuesta, string Parcela, string PrimerTitular);

public sealed record McpPropuestasPlaceResponse(
    int Nivel,
    int Lugares,
    string NombreInhumado1,
    string Estado1,
    string Color1,
    string NombreInhumado2,
    string Estado2,
    string Color2,
    string NombreInhumado3,
    string Estado3,
    string Color3,
    string NombreInhumado4,
    string Estado4,
    string Color4,
    string NombreInhumado5,
    string Estado5,
    string Color5,
    string NombreInhumado6,
    string Estado6,
    string Color6);

public sealed record McpPropuestasDeceasedResponse(
    int CodDetInhumado,
    string NombreInhumado,
    string TipoDocumento,
    int? NroDocumento,
    DateOnly? FechaNacimiento,
    DateOnly? FechaFallecimiento,
    DateOnly FechaInhumacion,
    DateOnly? FechaExhumacion,
    string NombreBim,
    int? NroDeclaracionJurada);

public sealed record McpPropuestasHolderResponse(
    int CodCliente,
    int Orden,
    string Nombre,
    string TipoDocumento,
    long NroDocumento,
    string Telefono,
    string TelefonoMovil,
    string Email,
    string Direccion,
    string Localidad,
    string Provincia,
    DateOnly? FechaNacimiento,
    DateOnly? FechaAlta,
    string Web);

public sealed record McpCurrentAccountResponse(
    string Tipo,
    bool EsRefinanciacion,
    bool EsDocumentado,
    string Categoria,
    string Estado,
    double Importe,
    double PrecioDolar,
    DateOnly FechaInicio,
    DateOnly? PrimerCuota,
    DateOnly? FechaBaja,
    string Cobrador,
    string Cliente,
    int CodCliente,
    string ZonaCobranza,
    string CobradorZona,
    int Codigo,
    int? CodMedioCobro,
    string Comercializadora,
    int Cuotas,
    DateOnly? CuotaDesde,
    DateOnly? CuotaHasta,
    double ImporteVencido,
    double Interes,
    double ImporteAVencer,
    double DescuentoVencido,
    double DescuentoAVencer,
    double Deuda,
    double Total,
    double PorcCancelado,
    int? CodCatRecargo,
    bool EsIndependiente,
    bool EsPrecioDiferencial);

public sealed record McpPropuestasServicesResponse(
    IReadOnlyList<McpEnabledServiceResponse> ServiciosHabilitados,
    IReadOnlyList<McpUsedServiceResponse> ServiciosUtilizados,
    IReadOnlyList<McpServiceBeneficiaryResponse> Beneficiarios);

public sealed record McpEnabledServiceResponse(
    string PlanBeneficio,
    string Cliente,
    IReadOnlyList<McpServiceMessageResponse> Mensajes,
    int CodCliente,
    int CantServicios);

public sealed record McpServiceMessageResponse(string Titulo, string Mensaje);

public sealed record McpUsedServiceResponse(string Servicio, int ServiciosRealizados, int ServiciosPendientes);

public sealed record McpServiceBeneficiaryResponse(
    DateOnly Fecha,
    string NroComprobante,
    string TipoServicio,
    int CodTalonario,
    string Nombre,
    int NroDocumento,
    string Producto,
    int UsadoDe,
    int UsadoEn);

public sealed record McpComprobantesResponse(
    IReadOnlyList<McpPropuestasComprobanteResponse> Comprobantes,
    int CantidadPaginas,
    int CantidadRegistros);

public sealed record McpPropuestasComprobanteResponse(
    DateOnly Fecha,
    string TipoComprobante,
    string Talonario,
    string NroComprobante,
    string Cliente,
    double Total,
    string Estado,
    string Concepto,
    string Anulado,
    double Pago,
    string Usuario,
    string NroComprobanteImputacion,
    string ClienteBeneficiado,
    int CodClienteBeneficiado,
    int CodTalonario,
    int CodTalonarioAsociado1,
    int CodMovimientoFondo);

public sealed record McpPropuestasContractsResponse(
    IReadOnlyList<McpPropuestasContractResponse> Contratos,
    IReadOnlyList<McpSalesPlanResponse> PlanesVenta,
    IReadOnlyList<McpPropuestasHolderResponse> Titulares,
    IReadOnlyList<McpInvoicePaymentHolderResponse> TitularesFacturasPagos);

public sealed record McpPropuestasContractResponse(
    int CodContrato,
    int? CodPlanVenta,
    DateOnly Fecha,
    DateOnly? FechaBaja,
    DateOnly? FechaCaducidad,
    string TipoContrato,
    string Modelo,
    string Vendedor,
    double Total);

public sealed record McpSalesPlanResponse(
    string PlanVenta,
    string Concepto,
    double Total,
    DateOnly VigenciaDesde,
    DateOnly? VigenciaHasta);

public sealed record McpInvoicePaymentHolderResponse(
    int CodCliente,
    bool Factura,
    bool Pago,
    string Nombre,
    string TipoDocumento,
    long NroDocumento,
    string Telefono,
    string TelefonoMovil);

public sealed record McpHolderHistoryResponse(
    string Nombre,
    DateTime FechaAlta,
    string AutorizaAlta,
    string UsuarioAlta,
    DateOnly? FechaBaja,
    string AutorizaBaja,
    string UsuarioBaja);
