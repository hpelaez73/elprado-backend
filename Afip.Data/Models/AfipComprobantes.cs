namespace Afip.Data.Models
{
    public class AfipComprobante
    {
        public int CodTalonario { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public int AfipTipoComprobante { get; set; }
        public int PuntoVenta { get; set; }
        public string Letra { get; set; } = string.Empty;
        public int AfipConcepto { get; set; }
        public int? AfipTipoDocumento { get; set; }
        public int? AfipCondicion { get; set; }
        public string? NroDocumento { get; set; }
        public long? Cuit { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public decimal Neto { get; set; }
        public decimal NoGravado { get; set; }
        public decimal Iva { get; set; }
        public DateTime? AfipServicioDesde { get; set; }
        public DateTime? AfipServicioHasta { get; set; }
    }

    public class AfipAlicuotas
    {
        public short IdIva { get; set; }
        public decimal Importe { get; set; }
        public decimal BaseImp { get; set; }
    }
}
