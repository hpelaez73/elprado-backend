namespace ElPrado.Dto.Dtos
{
    public class DtoPlanesVentasPropuestas
    {
        public string PlanVenta { get; set; } = string.Empty;
        public string Concepto { get; set; } = string.Empty;
        public double Total { get; set; }
        public DateOnly VigenciaDesde { get; set; }
        public DateOnly? VigenciaHasta { get; set; }
    }
}
