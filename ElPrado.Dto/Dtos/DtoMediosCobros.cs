namespace ElPrado.Dto.Dtos
{
    public class DtoMediosCobros : DtoBase
    {
        public string Tipo { get; set; } = string.Empty;
        public string LugarCobro { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
        public string ZonaCobranzas { get; set; } = string.Empty;
        public string Cobrador { get; set; } = string.Empty;
        public string InicialCobrador { get; set; } = string.Empty;
        public int CantEnvios { get; set; }
        public DateOnly? CantEnviosHasta { get; set; }
        public int EnviarElDia { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Localidad { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class DtoHistorialCobradores
    {
        public DateTime Fecha { get; set; }
        public string Cobrador { get; set; } = string.Empty;
    }
}
