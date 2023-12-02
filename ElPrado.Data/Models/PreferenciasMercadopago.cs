namespace ElPrado.Data.Models
{
    public class PreferenciasMercadopago : Entidades
    {
        [Key]
        public int CodPreferenciaMercadopago { get; set; }
        public string IdPreferencia { get; set; } = string.Empty;
        public string Referencia { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public long? IdPago { get; set; }
    }
}
