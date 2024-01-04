namespace ElPrado.Data.Models
{
    public class Comprobantes : Entidades
    {
        [Key]
        public int CodTalonario { get; set; }
        [Key]
        public string NroComprobante { get; set; } = string.Empty;
        public int CodTipoComprobante { get; set; }
        public int? CodCliente { get; set; }
        public int Codusuario { get; set; }
        public int? CodPropuesta { get; set; }
        public DateTime Fecha { get; set; }
        public double Total { get; set; }
    }
}
