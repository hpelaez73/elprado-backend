namespace ElPrado.Dto.Dtos
{
    public class DtoUsuarios : DtoBase
    {
        public int CodUsuario { get; set; }
        public string Alias { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string ClaveAcceso { get; set; } = string.Empty;
        public bool EsAdmin { get; set; }
        public bool SuperUsuario { get; set; }
        public bool PuedeAutorizar { get; set; }
    }
}
