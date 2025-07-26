namespace ElPrado.Dto
{
    public class DtoMovimientosFondos
    {
        public string TipoComprobante { get; set; } = string.Empty;
        public string Talonario { get; set; } = string.Empty;
        public string NroComprobante { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Alias { get; set; } = string.Empty;
        public string Concepto { get; set; } = string.Empty;
        public int? Propuesta { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string NroComprobanteReversion { get; set; } = string.Empty;
        public string ProcesadoPor { get; set; } = string.Empty;
        public double TotalDebe { get; set; }
        public double TotalHaber { get; set; }

        public List<DtoMovimientosFondosCuentas>? ListCuentas { get; set; }
        public List<DtoMovimientosFondosCupones>? ListCupones { get; set; }
        public List<DtoMovimientosFondosCheques>? ListCheques { get; set; }
    }

    public class DtoMovimientosFondosCuentas
    {
        public string TipoCuenta { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public double Unidades { get; set; }
        public double Debe { get; set; }
        public double Haber { get; set; }
    }

    public class DtoMovimientosFondosCupones
    {
        public string NumeroTarjeta { get; set; } = string.Empty;
        public string NumeroCupon { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public double Importe { get; set; }
        public int? Propuesta { get; set; }
        public string Titular { get; set; } = string.Empty;
        public string EmpresaTarjeta { get; set; } = string.Empty;
    }

    public class DtoMovimientosFondosCheques
    {
        public string NroCheque { get; set; } = string.Empty;
        public double Importe { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaCobro { get; set; }
        public string Banco { get; set; } = string.Empty;
        public int? Propuesta { get; set; }
    }
}
