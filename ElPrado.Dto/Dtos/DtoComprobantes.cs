namespace ElPrado.Dto.Dtos
{
    public class  DtoComprobanteReq
    {
        public int CodTalonario { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
    }

    public class DtoComprobantes
    {
        public DateTime Fecha { get; set; }
        public string TipoComprobante { get; set; } = string.Empty;
        public string Talonario { get; set; } = string.Empty;
        public string NroComprobante { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public int? NroDocumento { get; set; }
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
        public int? Propuesta { get; set; }
        public string Parcela { get; set; } = string.Empty;
        public string Manzana { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string ZonaCobranza { get; set; } = string.Empty;
        public string Cobrador { get; set; } = string.Empty;
        public string ClienteBeneficiado { get; set; } = string.Empty;
        public int? DocumentoBeneficiado { get; set; }
        public DateTime? FechaUtilizacion { get; set; }

        public List<DtoComprobantesDetalles>? Detalles { get; set; }
    }

    public class DtoComprobantesDetalles
    {
        public string Detalle { get; set; } = string.Empty;
        public double Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public double Total { get; set; }
        public double Porcentaje { get; set; }
    }

    public class DtoComprobantesPeriodo
    {
        public DateTime MinFecha { get; set; }
        public DateTime MaxFecha { get; set; }
    }

    public class DtoComprobantesFacturasElectronicas
    {
        public int Propuesta { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public double Total { get; set; }
        public string Link { get; set; } = string.Empty;
    }

    public class DtoFacturasEnviarList : DtoBase
    {
        public int Propuesta { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string TelefonoMovil { get; set; } = string.Empty;
        public string LinkWhatsApp { get; set; } = string.Empty;
        public DateTime? UltimoEnvio { get; set; }
        public int CodTalonario { get; set; }
        public int CodCliente { get; set; }
        public int CodPropuesta { get; set; }
    }

    public class DtoRegistrarEnvioReq
    {
        public int CodCliente { get; set; }
        public int CodTalonario { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public string TelefonoMovil { get; set; } = string.Empty;
    }
}
