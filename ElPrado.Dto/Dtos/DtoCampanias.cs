namespace ElPrado.Dto.Dtos
{
    public class DtoCampaniasPropuestaList : DtoBase
    {
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Campania { get; set; } = string.Empty;
        public string MotivoContacto { get; set; } = string.Empty;
        public string ResultadoContacto { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public string ColorWeb { get; set; } = string.Empty;
    }
}
