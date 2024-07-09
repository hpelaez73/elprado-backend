namespace ElPrado.Dto.Dtos
{
    public class DtoInhumados
    {
    }

    public class DtoInhumadosPropuestas
    {
        public int CodDetInhumado { get; set; }
        public string NombreInhumado { get; set; } = string.Empty;
        public string? TipoDocumento { get; set; }
        public int? NroDocumento { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public DateTime? FechaFallecimiento { get; set; }
        public DateTime FechaInhumacion { get; set; }
        public DateTime? FechaExhumacion { get; set; }
        public string? NombreBIM { get; set; }
        public int? NroDeclaracionJurada { get; set; }
    }
}
