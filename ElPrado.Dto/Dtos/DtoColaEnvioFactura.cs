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

    public class  DtoColaEnvioEstado : DtoBase
    {
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaUltimoIntento { get; set; }
        public int Intentos { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string Talonario { get; set; } = string.Empty;
        public string NroComprobante { get; set; } = string.Empty;
        public string MedioEnvio { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}
