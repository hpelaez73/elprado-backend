namespace ElPrado.Data.Models
{
    public class ComentariosObituarios : Entidades
    {
        [Key]
        public int CodComentario { get; set; }
        public int CodObituario { get; set; }
        public string Autor { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public bool Aprobado { get; set; }
        public int? CodComentarioPadre { get; set; }
    }
}
