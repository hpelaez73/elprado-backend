namespace ElPrado.Dto.Dtos
{
    public class DtoObituarios : DtoBase
    {
        public int CodObituario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateOnly? FechaNacimiento { get; set; }
        public DateOnly? FechaFallecimiento { get; set; }
        public string? UrlImagen { get; set; }
        public string? Biografia { get; set; }
        public bool PermitirCondolencias { get; set; }
        public bool ModerarCondolencias { get; set; }
        public bool PermitirComentarios { get; set; }
        public bool ModerarComentarios { get; set; }
    }

    public class DtoObituarioDetalleResp
    {
        public int CodObituario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateOnly? FechaNacimiento { get; set; }
        public DateOnly? FechaFallecimiento { get; set; }
        public string? UrlImagen { get; set; }
        public string? Biografia { get; set; }
        public bool PermitirCondolencias { get; set; }
        public bool ModerarCondolencias { get; set; }
        public bool PermitirComentarios { get; set; }
        public bool ModerarComentarios { get; set; }
        public List<DtoCondolenciasObituarios> Condolencias { get; set; } = new();
        public List<DtoComentariosObituarios> Comentarios { get; set; } = new();
        public List<DtoReaccionesObituarios> Reacciones { get; set; } = new();
        public List<DtoServicioObituarios> Servicios { get; set; } = new();
    }

    public class DtoReaccionesObituarios
    {
        public int CodReaccion { get; set; }
        public int CodObituario { get; set; }
        public string TipoReaccion { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class DtoServicioObituarios
    {
        public int CodServicio { get; set; }
        public int CodObituario { get; set; }
        public string TipoServicio { get; set; } = string.Empty;
        public string? Lugar { get; set; }
        public DateTime? FechaHora { get; set; }
        public string? UrlMaps { get; set; }
    }
}
