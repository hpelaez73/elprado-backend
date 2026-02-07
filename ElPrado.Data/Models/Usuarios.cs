namespace ElPrado.Data.Models
{
    public class Usuarios : Entidades
    {
        [Key]
        public int CodUsuario { get; set; }
        public string Alias { get; set; } = string.Empty;
        public string? Nombre { get; set; }
        public string ClaveAcceso { get; set; } = string.Empty;
        public bool EsAdmin { get; set; }
        public bool SuperUsuario { get; set; }
        public bool PuedeAutorizar { get; set; }
        public int? CodCliente { get; set; }
        public int? CodEmpleado { get; set; }
    }
}