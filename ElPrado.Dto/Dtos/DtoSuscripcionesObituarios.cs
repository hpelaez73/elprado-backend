namespace ElPrado.Dto.Dtos
{
    public class DtoSuscripcionesObituarios : DtoBase
    {
        public int CodSuscripcion { get; set; }
        public int CodObituario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public bool Aniversarios { get; set; }
        public bool Eventos { get; set; }
        public bool NuevosContenidos { get; set; }
    }
}