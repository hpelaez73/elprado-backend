namespace ElPrado.Dto.Dtos
{
    public class DtoColaEnvioFactura
    {
        public int CodColaEnvio { get; set; }
        public int CodTalonario { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public string MedioEnvio { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string UrlFactura { get; set; } = string.Empty;
        public string IdGuid { get; set; } = string.Empty;
        public string InformacionContacto1 { get; set; } = string.Empty;
        public string InformacionContacto2 { get; set; } = string.Empty;
        public string InformacionContacto3 { get; set; } = string.Empty;
        public string UrlWeb { get; set; } = string.Empty;
    }
}
