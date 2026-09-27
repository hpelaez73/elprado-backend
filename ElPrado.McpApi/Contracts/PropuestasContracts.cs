namespace ElPrado.McpApi.Contracts;

// Contratos de lectura exclusivos de la superficie v2. No reutilizan DTOs de
// WebApi ni los contratos v1, porque representan hechos de dominio para agentes.
public sealed record PropuestaDetalleResponse(
    int Propuesta,
    int Codigo,
    string? Tipo,
    DateOnly? FechaAlta,
    DateOnly? FechaBaja,
    string? Estado,
    EstadoDeuda? Deuda,
    Parcela? Parcela,
    IReadOnlyList<Inhumado> Inhumados,
    IReadOnlyList<PropuestaAsociada> PropuestasAsociadas,
    IReadOnlyList<string> Alertas);

public sealed record EstadoDeuda(
    string? Nombre,
    bool Activa,
    bool AlDia,
    bool PermiteImputar);

public sealed record Parcela(
    int Codigo,
    string? Numero,
    string? Manzana,
    EstadoParcela? Estado,
    IReadOnlyList<string> Zonas,
    IReadOnlyList<LugarParcela> Lugares);

public sealed record EstadoParcela(
    string? Nombre,
    bool Permanente,
    bool DisponibleVenta,
    bool DisponibleInhumar,
    bool Inhabilitada,
    bool ConInhumado);

public sealed record LugarParcela(
    int Nivel,
    int Posicion,
    EstadoLugar? Estado,
    InhumadoReferencia? Inhumado);

public sealed record EstadoLugar(
    string? Nombre,
    bool DisponibleVenta,
    bool DisponibleInhumar);

public sealed record InhumadoReferencia(int Codigo, string? Nombre);

public sealed record Inhumado(
    int Codigo,
    string? Nombre,
    Documento? Documento,
    DateOnly? FechaNacimiento,
    DateOnly? FechaFallecimiento,
    DateOnly? FechaInhumacion,
    DateOnly? FechaExhumacion);

public sealed record PropuestaAsociada(int Propuesta, string? Tipo);

public sealed record PropuestaTitularesResponse(
    int Propuesta,
    int Cantidad,
    IReadOnlyList<Titular> Titulares);

public sealed record Titular(
    int Codigo,
    int Orden,
    bool EsPrincipal,
    string? Nombre,
    Documento? Documento,
    Contacto? Contacto,
    Domicilio? Domicilio,
    DateOnly? FechaNacimiento,
    DateOnly? FechaAlta);

public sealed record Documento(string? Tipo, string? Numero);

public sealed record Contacto(string? Telefono, string? Movil, string? Email);

public sealed record Domicilio(string? Direccion, string? Localidad, string? Provincia);

public sealed record PropuestaDeudaResponse(
    int Propuesta,
    EstadoDeuda? Estado,
    TotalesDeuda Totales,
    IReadOnlyList<CuentaCorriente> Cuentas);

public sealed record TotalesDeuda(
    decimal Vencido,
    decimal Intereses,
    decimal AVencer,
    decimal DescuentoVencido,
    decimal DescuentoAVencer,
    decimal Deuda,
    decimal Total);

public sealed record CuentaCorriente(
    int Codigo,
    string? Tipo,
    string? Categoria,
    EstadoDeuda? Estado,
    ClienteReferencia? Cliente,
    decimal Importe,
    PeriodoCuenta? Periodo,
    TotalesDeuda Importes,
    Cobranza? Cobranza);

public sealed record ClienteReferencia(int Codigo, string? Nombre);

public sealed record PeriodoCuenta(
    DateOnly? Inicio,
    DateOnly? PrimeraCuota,
    DateOnly? CuotaDesde,
    DateOnly? CuotaHasta);

public sealed record Cobranza(string? Cobrador, string? Zona, string? Comercializadora);

public sealed record PropuestaServiciosResponse(
    int Propuesta,
    IReadOnlyList<HabilitacionServicio> Habilitaciones,
    IReadOnlyList<CupoServicio> Cupos,
    IReadOnlyList<UtilizacionServicio> Utilizaciones);

public sealed record HabilitacionServicio(
    ClienteReferencia? Cliente,
    string? Producto,
    string? Servicio,
    bool Habilitado,
    MotivoHabilitacion? Motivo);

public sealed record MotivoHabilitacion(string? Codigo, string? Descripcion, DateOnly? Hasta);

public sealed record CupoServicio(
    string? Producto,
    string? Servicio,
    int Total,
    int Utilizados,
    int Disponibles);

public sealed record UtilizacionServicio(
    DateOnly? Fecha,
    string? Producto,
    string? Servicio,
    BeneficiarioServicio? Beneficiario,
    ComprobanteReferencia? Comprobante,
    int? PropuestaOrigen,
    int? PropuestaAplicacion);

public sealed record BeneficiarioServicio(string? Nombre, Documento? Documento);

public sealed record ComprobanteReferencia(string? Numero);

public sealed record PropuestaContratosResponse(
    int Propuesta,
    IReadOnlyList<Contrato> Contratos,
    Facturacion Facturacion);

public sealed record Contrato(
    int Codigo,
    DateOnly? Fecha,
    string? Tipo,
    string? Modelo,
    string? Estado,
    decimal Total,
    Vendedor? Vendedor,
    PlanVenta? PlanVenta);

public sealed record Vendedor(string? Nombre);

public sealed record PlanVenta(int Codigo, string? Nombre, string? Concepto);

public sealed record Facturacion(IReadOnlyList<TitularFacturacion> TitularesHabilitados);

public sealed record TitularFacturacion(int Codigo, string? Nombre, bool Factura, bool Pago);

public sealed record PropuestaComprobantesResponse(
    int Propuesta,
    IReadOnlyList<Comprobante> Items,
    Paginacion Pagination);

public sealed record Comprobante(
    DateOnly? Fecha,
    string? Tipo,
    string? Numero,
    ClienteReferencia? Cliente,
    decimal Total,
    decimal Pago,
    string? Estado,
    bool Anulado);

public sealed record Paginacion(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasNext,
    bool HasPrevious);

public sealed record PropuestaBusquedaResponse(
    IReadOnlyList<PropuestaBusqueda> Items,
    Paginacion Pagination);

public sealed record PropuestaBusqueda(
    int Propuesta,
    string? Tipo,
    string? Estado,
    ParcelaBusqueda? Parcela,
    IReadOnlyList<TitularBusqueda> Titulares);

public sealed record ParcelaBusqueda(string? Numero);

public sealed record TitularBusqueda(string? Nombre, Documento? Documento);

public sealed record PropuestaHistorialTitularesResponse(
    int Propuesta,
    IReadOnlyList<HistorialTitular> Historial);

public sealed record HistorialTitular(
    ClienteReferencia? Cliente,
    DateOnly? FechaAlta,
    DateOnly? FechaBaja,
    string? UsuarioAlta,
    string? UsuarioBaja);
