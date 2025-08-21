namespace ElPrado.Dto.Dtos
{
    public class DtoServiciosPropuesta
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

    public class DtoServiciosUtilizadosPropuesta
    {
        public string Servicio { get; set; } = string.Empty;
        public int ServiciosRealizados { get; set; }
        public int ServiciosPendientes { get; set; }
    }

    public class DtoBeneficiariosPropuesta
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
