namespace ElPrado.Dto.Dtos
{
    public class DtoServiciosObituarios : DtoBase
    {
        public int CodServicio { get; set; }
        public int CodObituario { get; set; }
        public int CodTipoServicio { get; set; }
        public string TipoServicio { get; set; } = string.Empty;
        public string Lugar { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public string UrlMaps { get; set; } = string.Empty;
    }
}
