namespace ElPrado.Data.Models
{
    public class Obituarios : Entidades
    {
        [Key]
        public int CodObituario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public DateTime? FechaFallecimiento { get; set; }
        public string? UrlImagen { get; set; }
        public string? Biografia { get; set; }
        public bool PermitirCondolencias { get; set; }
        public bool ModerarCondolencias { get; set; }
        public bool PermitirComentarios { get; set; }
        public bool ModerarComentarios { get; set; }
    }
}
