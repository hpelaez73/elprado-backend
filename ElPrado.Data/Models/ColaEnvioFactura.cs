namespace ElPrado.Data.Models
{
    public class ColaEnvioFactura : Entidades
    {
        [Key]
        public int CodColaEnvio { get; set; }
        public int CodCliente { get; set; }
        public int CodTalonario { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public string MedioEnvio { get; set; } = string.Empty; // EMAIL / WHATSAPP
        public bool PorEmail { get; set; }
        public bool PorWhatsApp { get; set; }
        public string Estado { get; set; } = string.Empty; // PENDIENTE / ENVIANDO / ENVIADO / ERROR
        public int Intentos { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaUltimoIntento { get; set; }
        public string Error { get; set; } = string.Empty;
    }
}
