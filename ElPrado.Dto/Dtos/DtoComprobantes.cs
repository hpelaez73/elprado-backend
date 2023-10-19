using System.Reflection.Metadata.Ecma335;

namespace ElPrado.Dto.Dtos
{
    public class DtoComprobantes
    {
    }


    public class DtoComprobantesPeriodo
    {
        public DateTime MinFecha { get; set; }
        public DateTime MaxFecha { get; set; }
    }

    public class DtoComprobantesFacturasElectronicas
    {
        public int Propuesta { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public double Total { get; set; }
        public string Link { get; set; } = string.Empty;
    }
}
