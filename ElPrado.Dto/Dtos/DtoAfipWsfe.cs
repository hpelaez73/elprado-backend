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

    public class DtoAfipWsfeConsultaDetalle
    {
        public int CodTalonario { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public int Concepto { get; set; }
        public string ConceptoDescripcion { get; set; } = string.Empty;
        public int DocTipo { get; set; }
        public string DocTipoDescripcion { get; set; } = string.Empty;
        public long DocNro { get; set; }
        public int? CondicionIva { get; set; }
        public string CondicionIvaDescripcion { get; set; } = string.Empty;
        public long CbteDesde { get; set; }
        public long CbteHasta { get; set; }
        public string CbteFch { get; set; } = string.Empty;
        public double ImpTotal { get; set; }
        public double ImpTotConc { get; set; }
        public double ImpNeto { get; set; }
        public double ImpOpEx { get; set; }
        public double ImpTrib { get; set; }
        public double ImpIVA { get; set; }
        public string FchServDesde { get; set; } = string.Empty;
        public string FchServHasta { get; set; } = string.Empty;
        public string FchVtoPago { get; set; } = string.Empty;
        public string MonId { get; set; } = string.Empty;
        public string MonIdDescripcion { get; set; } = string.Empty;
        public double MonCotiz { get; set; }
        public string Resultado { get; set; } = string.Empty;
        public string ResultadoDescripcion { get; set; } = string.Empty;
        public string CodAutorizacion { get; set; } = string.Empty;
        public string EmisionTipo { get; set; } = string.Empty;
        public string EmisionTipoDescripcion { get; set; } = string.Empty;
        public string FchVto { get; set; } = string.Empty;
        public string FchProceso { get; set; } = string.Empty;
        public int PtoVta { get; set; }
        public int CbteTipo { get; set; }
        public string CbteTipoDescripcion { get; set; } = string.Empty;
        public List<DtoAfipWsfeCbteAsoc> CbtesAsoc { get; set; } = new();
        public List<DtoAfipWsfeTributo> Tributos { get; set; } = new();
        public List<DtoAfipWsfeIva> Iva { get; set; } = new();
        public List<DtoAfipWsfeOpcional> Opcionales { get; set; } = new();
        public List<DtoAfipWsfeComprador> Compradores { get; set; } = new();
        public List<DtoAfipWsfePeriodoAsoc> PeriodoAsoc { get; set; } = new();
        public List<DtoAfipWsfeObservacion> Observaciones { get; set; } = new();
        public List<DtoAfipWsfeError> Errors { get; set; } = new();
        public List<DtoAfipWsfeEvent> Events { get; set; } = new();
    }

    public class DtoAfipWsfeCbteAsoc
    {
        public int Tipo { get; set; }
        public string TipoDescripcion { get; set; } = string.Empty;
        public int PtoVta { get; set; }
        public long Nro { get; set; }
        public string Cuit { get; set; } = string.Empty;
        public string CbteFch { get; set; } = string.Empty;
    }

    public class DtoAfipWsfeTributo
    {
        public int Id { get; set; }
        public string IdDescripcion { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public double BaseImp { get; set; }
        public double Alic { get; set; }
        public double Importe { get; set; }
    }

    public class DtoAfipWsfeIva
    {
        public int Id { get; set; }
        public string IdDescripcion { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public double BaseImp { get; set; }
        public double Importe { get; set; }
    }

    public class DtoAfipWsfeOpcional
    {
        public int Id { get; set; }
        public string Valor { get; set; } = string.Empty;
    }

    public class DtoAfipWsfeComprador
    {
        public long DocNro { get; set; }
        public int DocTipo { get; set; }
        public string DocTipoDescripcion { get; set; } = string.Empty;
        public double Porcentaje { get; set; }
    }

    public class DtoAfipWsfePeriodoAsoc
    {
        public string FchDesde { get; set; } = string.Empty;
        public string FchHasta { get; set; } = string.Empty;
    }

    public class DtoAfipWsfeObservacion
    {
        public int Code { get; set; }
        public string Msg { get; set; } = string.Empty;
    }

    public class DtoAfipWsfeError
    {
        public int Code { get; set; }
        public string Msg { get; set; } = string.Empty;
    }

    public class DtoAfipWsfeEvent
    {
        public int Code { get; set; }
        public string Msg { get; set; } = string.Empty;
    }
}
