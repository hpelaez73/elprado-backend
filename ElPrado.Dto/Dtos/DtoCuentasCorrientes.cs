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

    public class DtoCuentasCorrientesReq
    {
        public string Tipo { get; set; } = string.Empty;
        public int Codigo { get; set; }
        public int CodCredito { get; set; }
        public int CodConfiguracion { get; set; }
    }

    public class DtoCuentasCorrientesResumenReq
    {
        public int CodPropuesta { get; set; }
        public bool MostrarBaja { get; set; }
        public bool MostrarInactiva { get; set; }
        public DateOnly? FechaInteres { get; set; }
        public DateOnly? FechaHasta { get; set; }
    }

    public class DtoCuentasCorrientesResumen
    {
        public string Tipo { get; set; } = string.Empty;
        public bool EsRefinanciacion { get; set; }
        public bool EsDocumentado { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public double Importe { get; set; }
        public double PrecioDolar { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly? PrimerCuota { get; set; }
        public DateOnly? FechaBaja { get; set; }
        public string Cobrador { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public int CodCliente { get; set; }
        public string ZonaCobranza { get; set; } = string.Empty;
        public string CobradorZona { get; set; } = string.Empty;
        public int Codigo { get; set; }
        public int? CodMedioCobro { get; set; }
        public string Comercializadora { get; set; } = string.Empty;
        public int Cuotas { get; set; }
        public DateOnly? CuotaDesde { get; set; }
        public DateOnly? CuotaHasta { get; set; }
        public double ImporteVencido { get; set; }
        public double Interes { get; set; }
        public double ImporteAVencer { get; set; }
        public double DescuentoVencido { get; set; }
        public double DescuentoAVencer { get; set; }
        public double Deuda { get; set; }
        public double Total { get; set; }
        public double PorcCancelado { get; set; }
        public int? CodCatRecargo { get; set; }
        public bool EsIndependiente { get; set; }
        public bool EsPrecioDiferencial { get; set; }
    }

    public class DtoCuotasResumenList : DtoBase
    {
        public string Tipo { get; set; } = string.Empty;
        public int CodCuenta { get; set; }
        public int Cuota { get; set; }
        public int Pago { get; set; }
        public DateOnly FechaCuota { get; set; }
        public double Total { get; set; }
        public DateOnly? FechaRendicion { get; set; }
        public int? CodTalonarioFacturacion { get; set; }
        public string NroComprobanteFacturacion { get; set; } = string.Empty;
        public int? CodTalonarioImputacion { get; set; }
        public string NroComprobanteImputacion { get; set; } = string.Empty;
        public double Monto { get; set; }
        public double Interes { get; set; }
        public double Descuento { get; set; }
        public double Punitorio { get; set; }
        public double DescuentoRecargo { get; set; }
        public bool Anulado { get; set; }
        public bool Inactivo { get; set; }
        public int? CodMovimientoFondo { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public bool EnTarjeta { get; set; }
        public bool EnCBU { get; set; }
        public bool Refinanciado { get; set; }
    }

    public class DtoCuotasMercadoPago
    {
        public string Tipo { get; set; } = string.Empty;
        public int Codigo { get; set; }
        public int Cuota { get; set; }
        public int Pago { get; set; }
    }

    public class DtoSolicitudMercadoPago
    {
        public int CodCliente { get; set; }
        public DateTime VencimientoLink { get; set; }
        public List<DtoCuotasMercadoPago> ListCuotas { get; set; } = new();
    }
}
