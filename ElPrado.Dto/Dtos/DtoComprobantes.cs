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

    public class DtoFacturasEnviarList : DtoBase
    {
        public int Propuesta { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string TelefonoMovil { get; set; } = string.Empty;
        public string LinkWhatsApp { get; set; } = string.Empty;
        public int CodTalonario { get; set; }
        public int CodCliente { get; set; }
        public int CodPropuesta { get; set; }
    }

    public class DtoRegistrarEnvioReq
    {
        public int CodCliente { get; set; }
        public int CodTalonario { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public string TelefonoMovil { get; set; } = string.Empty;
    }
}
