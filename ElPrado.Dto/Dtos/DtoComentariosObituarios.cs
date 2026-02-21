namespace ElPrado.Dto.Dtos
{
    public class DtoComentariosObituarios : DtoBase
    {
        public int CodComentario { get; set; }
        public int CodObituario { get; set; }
        public string Autor { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public bool Aprobado { get; set; }
        public string UrlImagen1 { get; set; } = string.Empty;
        public string UrlImagen2 { get; set; } = string.Empty;
        public string UrlImagen3 { get; set; } = string.Empty;
        public int? CodComentarioPadre { get; set; }
    }
}
