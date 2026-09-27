namespace ElPrado.Dto.Dtos
{
    public class DtoServiciosPropuesta
    {
        public List<DtoServiciosHabilitados>? ListServiciosHabilitados { get; set; }
        public List<DtoServiciosUtilizados>? ListServiciosUtilizados { get; set; }
        public List<DtoServiciosBeneficiarios>? ListServiciosBeneficiarios { get; set; }
    }

    /// <summary>Hechos planos de servicios calculados por el ERP para la fachada v2.</summary>
    public class DtoServicioPropuestaV2Fuente
    {
        public int CodServicioModelo { get; set; }
        public int Orden { get; set; }
        public int? CodPlanVenta { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Servicio { get; set; } = string.Empty;
        public bool EnMora { get; set; }
        public bool InhabilitadoPorMora { get; set; }
        public bool Habilitado { get; set; }
        public DateOnly? VigenciaDesde { get; set; }
        public DateOnly? VigenciaHasta { get; set; }
        public DateOnly? SuspendidoHasta { get; set; }
        public int LimiteServicios { get; set; }
        public int ServiciosRealizados { get; set; }
        public int ServiciosPendientes { get; set; }
        public int UtilizadoTitular { get; set; }
        public int? CodCliente { get; set; }
        public string? Cliente { get; set; }
        public bool MayorEdad { get; set; }
        public int? CodInhumado { get; set; }
        public DateOnly? FechaCaducidad { get; set; }
    }

    public class DtoServiciosPropuestaV2
    {
        public List<DtoHabilitacionServicioV2> Habilitaciones { get; init; } = new();
        public List<DtoCupoServicioV2> Cupos { get; init; } = new();
        public List<DtoUtilizacionServicioV2> Utilizaciones { get; init; } = new();
    }

    public class DtoHabilitacionServicioV2
    {
        public int? CodCliente { get; init; }
        public string? Cliente { get; init; }
        public string Producto { get; init; } = string.Empty;
        public string Servicio { get; init; } = string.Empty;
        public bool Habilitado { get; init; }
        public string? MotivoCodigo { get; init; }
        public string? MotivoDescripcion { get; init; }
        public DateOnly? MotivoHasta { get; init; }
    }

    public class DtoCupoServicioV2
    {
        public string Producto { get; init; } = string.Empty;
        public string Servicio { get; init; } = string.Empty;
        public int Total { get; init; }
        public int Utilizados { get; init; }
        public int Disponibles { get; init; }
    }

    public class DtoUtilizacionServicioV2
    {
        public DateOnly? Fecha { get; set; }
        public string? Producto { get; set; }
        public string? Servicio { get; set; }
        public int? CodCliente { get; set; }
        public string? Beneficiario { get; set; }
        public string? TipoDocumento { get; set; }
        public long? NroDocumento { get; set; }
        public string? NroComprobante { get; set; }
        public int? PropuestaOrigen { get; set; }
        public int? PropuestaAplicacion { get; set; }
    }

    public class DtoServiciosHabilitados
    {
        public string PlanBeneficio { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string TituloServicio01 { get; set; } = string.Empty;
        public string MensajeServicio01 { get; set; } = string.Empty;
        public string TituloServicio02 { get; set; } = string.Empty;
        public string MensajeServicio02 { get; set; } = string.Empty;
        public string TituloServicio03 { get; set; } = string.Empty;
        public string MensajeServicio03 { get; set; } = string.Empty;
        public string TituloServicio04 { get; set; } = string.Empty;
        public string MensajeServicio04 { get; set; } = string.Empty;
        public string TituloServicio05 { get; set; } = string.Empty;
        public string MensajeServicio05 { get; set; } = string.Empty;
        public string TituloServicio06 { get; set; } = string.Empty;
        public string MensajeServicio06 { get; set; } = string.Empty;
        public string TituloServicio07 { get; set; } = string.Empty;
        public string MensajeServicio07 { get; set; } = string.Empty;
        public string TituloServicio08 { get; set; } = string.Empty;
        public string MensajeServicio08 { get; set; } = string.Empty;
        public string TituloServicio09 { get; set; } = string.Empty;
        public string MensajeServicio09 { get; set; } = string.Empty;
        public string TituloServicio10 { get; set; } = string.Empty;
        public string MensajeServicio10 { get; set; } = string.Empty;
        public string TituloServicio11 { get; set; } = string.Empty;
        public string MensajeServicio11 { get; set; } = string.Empty;
        public string TituloServicio12 { get; set; } = string.Empty;
        public string MensajeServicio12 { get; set; } = string.Empty;
        public string TituloServicio13 { get; set; } = string.Empty;
        public string MensajeServicio13 { get; set; } = string.Empty;
        public string TituloServicio14 { get; set; } = string.Empty;
        public string MensajeServicio14 { get; set; } = string.Empty;
        public string TituloServicio15 { get; set; } = string.Empty;
        public string MensajeServicio15 { get; set; } = string.Empty;
        public int CodCliente { get; set; }
        public int CantServicios { get; set; }
    }

    public class DtoServiciosUtilizados
    {
        public string Servicio { get; set; } = string.Empty;
        public int ServiciosRealizados { get; set; }
        public int ServiciosPendientes { get; set; }
    }

    public class DtoServiciosBeneficiarios
    {
        public DateOnly Fecha { get; set; }
        public string NroComprobante { get; set; } = string.Empty;
        public string TipoServicio { get; set; } = string.Empty;
        public int CodTalonario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int NroDocumento { get; set; }
        public string Producto { get; set; } = string.Empty;
        public int UsadoDe { get; set; }
        public int UsadoEn { get; set; }
    }
}
