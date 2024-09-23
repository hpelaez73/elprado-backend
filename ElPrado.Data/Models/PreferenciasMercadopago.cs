namespace ElPrado.Data.Models
{
    public class PreferenciasMercadopago : Entidades
    {
        [Key]
        public int CodPreferenciaMercadopago { get; set; }
        public string IdPreferencia { get; set; } = string.Empty;
        public int CodCliente { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public long? IdPago { get; set; }
    }

    public class DetPreferenciasMercadopagoCr
    {
        public int CodPreferenciaMercadopago { get; set; }
        public int CodCredito { get; set; }
        public int Cuota { get; set; }
        public int Pago { get; set; }
        public double Importe { get; set; }
    }

    public class DetPreferenciasMercadopagoCp
    {
        public int CodPreferenciaMercadopago { get; set; }
        public int CodConfiguracion { get; set; }
        public int Cuota { get; set; }
        public int Pago { get; set; }
        public double Importe { get; set; }
    }
}
