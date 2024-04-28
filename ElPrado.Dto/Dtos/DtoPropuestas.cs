namespace ElPrado.Dto.Dtos
{
    public class DtoPropuestasTitulares
    {
        public int CodPropuesta { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public long NroDocumento { get; set; }
        public int Propuesta { get; set; }
        public string Parcela { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
    public class DtoPropuestasInhumados
    {
        public int CodPropuesta { get; set; }
        public string NombreInhumado { get; set; } = string.Empty;
        public long NroDocumento { get; set; }
        public DateTime? FechaInhumacion { get; set; }
        public int Propuesta { get; set; }
        public string Parcela { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}

