namespace ElPrado.Dto.Dtos
{
    public class  DtoComprobanteReq
    {
        public int CodTalonario { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
    }

    public class DtoComprobantes
    {
        public DateOnly Fecha { get; set; }
        public string TipoComprobante { get; set; } = string.Empty;
        public string Talonario { get; set; } = string.Empty;
        public string NroComprobante { get; set; } = string.Empty;
        public string Letra { get; set; } = string.Empty;
        public int PuntoVenta { get; set; }

        public int? CodigoAfip { get; set; }
        public string AfipResultado { get; set; } = string.Empty;
        public string AfipCAE { get; set; } = string.Empty;
        public DateTime? AfipVencimientoCAE { get; set; }
        public int? AfipTipoDocumento { get; set; }
        public long? AfipNroDocumento { get; set; }

        public string TipoDocumento { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public long? NroDocumento { get; set; }
        public long? Cuit { get; set; }
        public string Telefono { get; set; } = string.Empty;
        public string CategoriaIva { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Localidad { get; set; } = string.Empty;

        public double Neto { get; set; }
        public double Redondeo { get; set; }
        public double NoGravado { get; set; }
        public double Iva { get; set; }
        public double Total { get; set; }
        public bool Pagado { get; set; }
        public string Observaciones { get; set; } = string.Empty;

        public int? Propuesta { get; set; }
        public string Parcela { get; set; } = string.Empty;
        public string Manzana { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string ZonaCobranza { get; set; } = string.Empty;
        public string Cobrador { get; set; } = string.Empty;

        public string ClienteBeneficiado { get; set; } = string.Empty;
        public int? DocumentoBeneficiado { get; set; }
        public DateOnly? FechaUtilizacion { get; set; }

        public List<DtoComprobantesDetalles>? ListDetalles { get; set; }
    }

    public class DtoComprobantesDetalles
    {
        public string Detalle { get; set; } = string.Empty;
        public double? Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public double Total { get; set; }
        public double Porcentaje { get; set; }
    }

    public class DtoComprobantesPeriodo
    {
        public DateOnly MinFecha { get; set; }
        public DateOnly MaxFecha { get; set; }
    }

    public class DtoComprobantesFacturasElectronicas
    {
        public int Propuesta { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public DateOnly Fecha { get; set; }
        public double Total { get; set; }
        public string NombrePdf { get; set; } = string.Empty;
        public int CodTalonario { get; set; }
    }

    public class DtoComprobantesFacturasPublicas
    {
        public int Anio { get; set; }
        public string NombrePdf { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public int CodTalonario { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
    }

    public class DtoFacturasEnviarList : DtoBase
    {
        public int Propuesta { get; set; }
        public int PropuestaFact { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public DateOnly Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public double Total { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateOnly? UltimoEnvio { get; set; }
        public int CodTalonario { get; set; }
        public int CodCliente { get; set; }
    }

    public class DtoRegistrarEnvioListReq
    {
        public List<DtoRegistrarEnvioReq>? ListComprobantes { get; set; }
    }

    public class DtoRegistrarEnvioReq
    {
        public int CodCliente { get; set; }
        public int CodTalonario { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class DtoComprobantesPropuestaList : DtoBase
    {
        public DateOnly Fecha { get; set; }
        public string TipoComprobante { get; set; } = string.Empty;
        public string Talonario { get; set; } = string.Empty;
        public string NroComprobante { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public double Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Concepto { get; set; } = string.Empty;
        public string Anulado { get; set; } = string.Empty;
        public double Pago { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string NroComprobanteImputacion { get; set; } = string.Empty;
        public string ClienteBeneficiado { get; set; } = string.Empty;
        public int CodClienteBeneficiado { get; set; }
        public int CodTalonario { get; set; }
        public int CodTalonarioAsociado1 { get; set; }
        public int CodMovimientoFondo { get; set; }
    }

}
