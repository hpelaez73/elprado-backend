namespace ElPrado.Data.Models
{
    public class Clientes : Entidades
    {
        [Key]
        public int CodCliente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public long? NroDocumento { get; set; }
        public string Telefono { get; set; } = string.Empty;
        public string TelefonoMovil { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ClaveAcceso { get; set; } = string.Empty;
    }
}
