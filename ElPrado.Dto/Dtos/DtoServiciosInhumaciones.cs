namespace ElPrado.Dto.Dtos
{
    public class DtoServiciosInhumacionesResp
    {
        public string Fallecido { get; set; } = string.Empty;
        public DateOnly FechaServicio { get; set; }
        public TimeOnly HoraServicio { get; set; }
        public string Parcela { get; set; } = string.Empty;
        public string Sala { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
