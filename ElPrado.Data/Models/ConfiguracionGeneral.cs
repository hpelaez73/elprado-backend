namespace ElPrado.Data.Models
{
    public class ConfiguracionGeneral : Entidades
    {
        public int CodConfiguracion { get; set; }
        public string Macro { get; set; } = string.Empty;
        public string? ValorCaracter { get; set; }
        public DateTime? ValorFecha { get; set; }
        public bool ValorBooleano { get; set; }
        public double? ValorNumerico { get; set; }
        public int? ValorEntero { get; set; }
        public double? ValorFloat { get; set; }
    }
}
