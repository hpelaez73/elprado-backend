namespace ElPrado.Data.Models
{
    public class Clientes : Entidades
    {
        [Key]
        public int CodCliente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? TipoDocumento { get; set; }
        public long? NroDocumento { get; set; }
        public long? Cuit { get; set; }
        public string? Telefono { get; set; }
        public string? TelefonoMovil { get; set; }
        public string? Email { get; set; }
        public string? ClaveAcceso { get; set; }
        public int CodCatIva {  get; set; }
    }
}
