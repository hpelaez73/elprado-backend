namespace ElPrado.Data.Models
{
    public class RecuperacionesClave : Entidades
    {
        [Key]
        public int CodRecuperacion { get; set; }
        public int CodCliente { get; set; }
        public int Propuesta { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaUtilizacion { get; set; }
        public string? IpSolicitud { get; set; }
    }
}
