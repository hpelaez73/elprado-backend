namespace ElPrado.Data.Models
{
    public class Usuarios : Entidades
    {
        [Key]
        public int CodUsuario { get; set; }
        public string Alias { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string ClaveAcceso { get; set; } = string.Empty;
    }
}
