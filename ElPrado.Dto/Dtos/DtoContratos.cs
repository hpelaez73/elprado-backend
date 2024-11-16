namespace ElPrado.Dto.Dtos
{
    public class DtoContratosPropuestas
    {
        public int CodContrato { get; set; }
        public int? CodPlanVenta { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? FechaBaja { get; set; }
        public DateTime? FechaCaducidad { get; set; }
        public string TipoContrato { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Vendedor { get; set; } = string.Empty;
        public double Total { get; set; }
    }
}
