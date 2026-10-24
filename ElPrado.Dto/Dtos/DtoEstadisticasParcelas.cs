namespace ElPrado.Dto.Dtos
{
    public sealed class DtoKpiParcelas
    {
        public int Parcelas { get; set; }
        public int AlDia { get; set; }
        public int Morosas { get; set; }
        public int Recuperadas { get; set; }
        public decimal? PorcentajeAlDia => Parcelas == 0 ? null : decimal.Round((decimal)AlDia * 100 / Parcelas, 2);
        public decimal? PorcentajeMorosas => Parcelas == 0 ? null : decimal.Round((decimal)Morosas * 100 / Parcelas, 2);
    }

    public sealed class DtoEstadisticasObservacion
    {
        public string Periodo { get; set; } = string.Empty;
        public DateTime FechaCorte { get; set; }
        public DateTime ActualizadoEn { get; set; }
        public DtoKpiParcelas Kpi { get; set; } = new();
    }

    public sealed class DtoEstadisticasResumen
    {
        public bool DatosDisponibles { get; set; }
        public string Periodo { get; set; } = string.Empty;
        public DateTime? PeriodoCorte { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public string Producto { get; set; } = "todos";
        public DtoKpiParcelas? Kpi { get; set; }
    }

    public sealed class DtoEstadisticasEvolucion
    {
        public string Desde { get; set; } = string.Empty;
        public string Hasta { get; set; } = string.Empty;
        public string Producto { get; set; } = "todos";
        public List<DtoEstadisticasObservacion> Observaciones { get; set; } = new();
        public List<string> PeriodosFaltantes { get; set; } = new();
    }

    // Tipo interno de transporte para los procedimientos Firebird.
    public sealed class DtoEstadisticasFila
    {
        public DateTime Periodo { get; set; }
        public DateTime FechaCorte { get; set; }
        public DateTime ActualizadoEn { get; set; }
        public int CantidadParcelas { get; set; }
        public int AlDia { get; set; }
        public int Morosas { get; set; }
        public int Recuperadas { get; set; }
    }

    public sealed class DtoEstadisticasCobertura
    {
        public DateTime Periodo { get; set; }
        public DateTime FechaCorte { get; set; }
        public DateTime ActualizadoEn { get; set; }
    }
}
