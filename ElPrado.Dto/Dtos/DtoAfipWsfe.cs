namespace ElPrado.Dto.Dtos
{
    public class DtoAfipWsfe
    {
        public int CodTalonario { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public string NroCAE { get; set; } = string.Empty;
    }

    public class DtoAfipWsfeResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int CodTalonario { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public string NroCAE { get; set; } = string.Empty;
    }

    public class DtoAfipWsfeUltimoReq
    {
        public int CodTipoComprobante { get; set; }
        public int CodTalonario { get; set; }
    }

    public class DtoAfipWsfeSolicitarReq
    {
        public int CodTalonario { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
    }
}
