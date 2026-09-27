namespace ElPrado.McpApi.Contracts;

// Contratos de lectura exclusivos de la superficie v2. No reutilizan DTOs de
// WebApi ni los contratos v1, porque representan hechos de dominio para agentes.
public sealed record PropuestaV2DetalleResponse(
    int Propuesta,
    int Codigo,
    string? Tipo,
    DateOnly? FechaAlta,
    DateOnly? FechaBaja,
    string? Estado,
    EstadoDeudaV2? Deuda,
    ParcelaV2? Parcela,
    IReadOnlyList<InhumadoV2> Inhumados,
    IReadOnlyList<PropuestaAsociadaV2> PropuestasAsociadas,
    IReadOnlyList<string> Alertas);

public sealed record EstadoDeudaV2(
    string? Nombre,
    bool Activa,
    bool AlDia,
    bool PermiteImputar);

public sealed record ParcelaV2(
    int Codigo,
    string? Numero,
    string? Manzana,
    EstadoParcelaV2? Estado,
    IReadOnlyList<string> Zonas,
    IReadOnlyList<LugarParcelaV2> Lugares);

public sealed record EstadoParcelaV2(
    string? Nombre,
    bool Permanente,
    bool DisponibleVenta,
    bool DisponibleInhumar,
    bool Inhabilitada,
    bool ConInhumado);

public sealed record LugarParcelaV2(
    int Nivel,
    int Posicion,
    EstadoLugarV2? Estado,
    InhumadoReferenciaV2? Inhumado);

public sealed record EstadoLugarV2(
    string? Nombre,
    bool DisponibleVenta,
    bool DisponibleInhumar);

public sealed record InhumadoReferenciaV2(int Codigo, string? Nombre);

public sealed record InhumadoV2(
    int Codigo,
    string? Nombre,
    DocumentoV2? Documento,
    DateOnly? FechaNacimiento,
    DateOnly? FechaFallecimiento,
    DateOnly? FechaInhumacion,
    DateOnly? FechaExhumacion);

public sealed record PropuestaAsociadaV2(int Propuesta, string? Tipo);

public sealed record PropuestaV2TitularesResponse(
    int Propuesta,
    int Cantidad,
    IReadOnlyList<TitularV2> Titulares);

public sealed record TitularV2(
    int Codigo,
    int Orden,
    bool EsPrincipal,
    string? Nombre,
    DocumentoV2? Documento,
    ContactoV2? Contacto,
    DomicilioV2? Domicilio,
    DateOnly? FechaNacimiento,
    DateOnly? FechaAlta);

public sealed record DocumentoV2(string? Tipo, string? Numero);

public sealed record ContactoV2(string? Telefono, string? Movil, string? Email);

public sealed record DomicilioV2(string? Direccion, string? Localidad, string? Provincia);

public sealed record PropuestaV2DeudaResponse(
    int Propuesta,
    EstadoDeudaV2? Estado,
    TotalesDeudaV2 Totales,
    IReadOnlyList<CuentaCorrienteV2> Cuentas);

public sealed record TotalesDeudaV2(
    decimal Vencido,
    decimal Intereses,
    decimal AVencer,
    decimal DescuentoVencido,
    decimal DescuentoAVencer,
    decimal Deuda,
    decimal Total);

public sealed record CuentaCorrienteV2(
    int Codigo,
    string? Tipo,
    string? Categoria,
    EstadoDeudaV2? Estado,
    ClienteReferenciaV2? Cliente,
    decimal Importe,
    PeriodoCuentaV2? Periodo,
    TotalesDeudaV2 Importes,
    CobranzaV2? Cobranza);

public sealed record ClienteReferenciaV2(int Codigo, string? Nombre);

public sealed record PeriodoCuentaV2(
    DateOnly? Inicio,
    DateOnly? PrimeraCuota,
    DateOnly? CuotaDesde,
    DateOnly? CuotaHasta);

public sealed record CobranzaV2(string? Cobrador, string? Zona, string? Comercializadora);

public sealed record PropuestaV2ServiciosResponse(
    int Propuesta,
    IReadOnlyList<HabilitacionServicioV2> Habilitaciones,
    IReadOnlyList<CupoServicioV2> Cupos,
    IReadOnlyList<UtilizacionServicioV2> Utilizaciones);

public sealed record HabilitacionServicioV2(
    ClienteReferenciaV2? Cliente,
    string? Producto,
    string? Servicio,
    bool Habilitado,
    MotivoHabilitacionV2? Motivo);

public sealed record MotivoHabilitacionV2(string? Codigo, string? Descripcion, DateOnly? Hasta);

public sealed record CupoServicioV2(
    string? Producto,
    string? Servicio,
    int Total,
    int Utilizados,
    int Disponibles);

public sealed record UtilizacionServicioV2(
    DateOnly? Fecha,
    string? Producto,
    string? Servicio,
    BeneficiarioServicioV2? Beneficiario,
    ComprobanteReferenciaV2? Comprobante,
    int? PropuestaOrigen,
    int? PropuestaAplicacion);

public sealed record BeneficiarioServicioV2(string? Nombre, DocumentoV2? Documento);

public sealed record ComprobanteReferenciaV2(string? Numero);

public sealed record PropuestaV2ContratosResponse(
    int Propuesta,
    IReadOnlyList<ContratoV2> Contratos,
    FacturacionV2 Facturacion);

public sealed record ContratoV2(
    int Codigo,
    DateOnly? Fecha,
    string? Tipo,
    string? Modelo,
    string? Estado,
    decimal Total,
    VendedorV2? Vendedor,
    PlanVentaV2? PlanVenta);

public sealed record VendedorV2(string? Nombre);

public sealed record PlanVentaV2(int Codigo, string? Nombre, string? Concepto);

public sealed record FacturacionV2(IReadOnlyList<TitularFacturacionV2> TitularesHabilitados);

public sealed record TitularFacturacionV2(int Codigo, string? Nombre, bool Factura, bool Pago);

public sealed record PropuestaV2ComprobantesResponse(
    int Propuesta,
    IReadOnlyList<ComprobanteV2> Items,
    PaginacionV2 Pagination);

public sealed record ComprobanteV2(
    DateOnly? Fecha,
    string? Tipo,
    string? Numero,
    ClienteReferenciaV2? Cliente,
    decimal Total,
    decimal Pago,
    string? Estado,
    bool Anulado);

public sealed record PaginacionV2(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasNext,
    bool HasPrevious);

public sealed record PropuestaV2HistorialTitularesResponse(
    int Propuesta,
    IReadOnlyList<HistorialTitularV2> Historial);

public sealed record HistorialTitularV2(
    ClienteReferenciaV2? Cliente,
    DateOnly? FechaAlta,
    DateOnly? FechaBaja,
    string? UsuarioAlta,
    string? UsuarioBaja);
