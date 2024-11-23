namespace ElPrado.Dto.Dtos
{
    public class DtoPlanesVentasPropuestas
    {
        public string PlanVenta { get; set; } = string.Empty;
        public string Concepto { get; set; } = string.Empty;
        public double Total { get; set; }
        public DateTime VigenciaDesde { get; set; }
        public DateTime? VigenciaHasta { get; set; }
    }
}
