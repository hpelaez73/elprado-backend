namespace ElPrado.Dto.Dtos
{
    public class DtoCuentasCorrientes
    {
        public int Propuesta { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int Codigo { get; set; }
        public int Cuota { get; set; }
        public int Pago { get; set; }
        public DateTime FechaCuota { get; set; }
        public double Total { get; set; }
        public int CodGrupo { get; set; }
        public bool EsIndependiente { get; set; }
        public bool MedioCobroHabilitado { get; set; }
        public bool ConDeudaGrande { get; set; }
    }

    public class DtoCuotasMercadoPago
    {
        public string Tipo { get; set; } = string.Empty;
        public int Codigo { get; set; }
        public int Cuota { get; set; }
        public int Pago { get; set; }
    }
}
