namespace ElPrado.Dto.Dtos
{
    public class DtoPropuestaDetalleReq
    {
        public int? Propuesta { get; set; }
        public string? Parcela { get; set; }
        public bool IncluirBaja { get; set; }
    }

    public class DtoPropuestaDetalleResp
    {
        // Datos de la propuesta
        public int CodPropuesta { get; set; }
        public int Propuesta { get; set; }
        public string TipoPropuesta { get; set; } = string.Empty;
        public DateOnly Fecha { get; set; }
        public DateOnly? FechaBaja { get; set; }
        public string EstadoDeuda { get; set; } = string.Empty;
        public bool MuestraMensajeAlerta { get; set; }
        public string MensajeAlerta { get; set; } = string.Empty;
        public List<DtoPropuestasAsociadas>? ListPropuestasAsociadas { get; set; }

        // Datos de la parcela
        public int? CodParcela { get; set; }
        public string Parcela { get; set; } = string.Empty;
        public string Manzana { get; set; } = string.Empty;
        public string EstadoParcela { get; set; } = string.Empty;
        public DtoEstadoParcela? EstadoParcelaDetalle { get; set; }
        public List<string>? ListZonasParcelas { get; set; }
        public List<DtoParcelasDetallesLugares>? ListDetalleLugares { get; set; }
        public List<DtoParcelaLugar>? ListLugares { get; set; }

        // Datos de los inhumados
        public List<DtoInhumadosPropuestas>? ListInhumados { get; set; }
    }

    public class DtoPropuestaDetalleContratosResp
    {
        // Datos de los contratos
        public List<DtoContratosPropuestas>? ListContratos { get; set; }

        // Datos de los detalles de planes de ventas
        public List<DtoPlanesVentasPropuestas>? ListPlanesVentas { get; set; }

        // Datos de los titulares
        public List<DtoClientesPropuestas>? ListTitulares { get; set; }

        // Datos de los titulares que pagan
        public List<DtoClientesPropuestasFacturasPagos>? ListTitularesFacturasPagos { get; set; }
    }

    public class DtoPropuestasAsociadas
    {
        public int CodPropuesta { get; set; }
        public int Propuesta { get; set; }
        public string Parcela { get; set; } = string.Empty;
        public string PrimerTitular { get; set; } = string.Empty;
    }

    /// <summary>Hechos funcionales del estado configurable de una parcela.</summary>
    public class DtoEstadoParcela
    {
        public string Nombre { get; set; } = string.Empty;
        public bool Permanente { get; set; }
        public bool DisponibleVenta { get; set; }
        public bool DisponibleInhumar { get; set; }
        public bool Inhabilitada { get; set; }
        public bool ConInhumado { get; set; }
    }

    public class DtoPropuestasTitularesList : DtoBase
    {
        public int CodPropuesta { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public long NroDocumento { get; set; }
        public int Propuesta { get; set; }
        public string Parcela { get; set; } = string.Empty;
        public DateOnly Fecha { get; set; }
        public DateOnly? FechaBaja { get; set; }
        public int CodCliente { get; set; }
    }

    public class DtoPropuestasInhumadosList : DtoBase
    {
        public int CodPropuesta { get; set; }
        public string NombreInhumado { get; set; } = string.Empty;
        public long NroDocumento { get; set; }
        public DateOnly? FechaInhumacion { get; set; }
        public int Propuesta { get; set; }
        public string Parcela { get; set; } = string.Empty;
        public DateOnly Fecha { get; set; }
        public DateOnly? FechaBaja { get; set; }
    }
}

