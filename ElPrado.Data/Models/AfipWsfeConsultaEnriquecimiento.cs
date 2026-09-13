namespace ElPrado.Data.Models
{
    public class AfipWsfeConsultaEnriquecimiento
    {
        public int? Concepto { get; set; }
        public int? TipoComprobante { get; set; }
        public string TipoComprobanteDescripcion { get; set; } = string.Empty;
        public long? Cuit { get; set; }
        public long? NroDocumento { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public int? CondicionIva { get; set; }
        public string CondicionIvaDescripcion { get; set; } = string.Empty;
        public IReadOnlyDictionary<int, string> TiposDocumentos { get; set; } = new Dictionary<int, string>();
        public IReadOnlyDictionary<int, string> TiposComprobantes { get; set; } = new Dictionary<int, string>();
        public IReadOnlyDictionary<int, string> TiposIva { get; set; } = new Dictionary<int, string>();
        public IReadOnlyDictionary<int, string> TiposTributos { get; set; } = new Dictionary<int, string>();
    }

    public class AfipCodigoDescripcion
    {
        public int Codigo { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
