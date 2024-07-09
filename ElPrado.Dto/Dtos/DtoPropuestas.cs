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
        public DateTime Fecha { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string EstadoDeuda { get; set; } = string.Empty;
        public bool MuestraMensajeAlerta { get; set; }
        public string MensajeAlerta { get; set; } = string.Empty;

        // Datos de la parcela
        public int? CodParcela { get; set; }
        public string Parcela { get; set; } = string.Empty;
        public string Manzana { get; set; } = string.Empty;
        public string EstadoParcela { get; set; } = string.Empty;
        public DtoCoordenadas? Coordenada { get; set; }
        public List<string>? ListZonasParcelas { get; set; }
        public List<DtoParcelasDetallesLugares>? ListDetalleLugares { get; set; }

        // Datos de los inhumados
        public List<DtoInhumadosPropuestas>? ListInhumados { get; set; }

        // Datos de los titulares
        public List<DtoClientesPropuestas>? ListTitulares { get; set; }
    }

    public class DtoPropuestasTitularesList : DtoBase
    {
        public int CodPropuesta { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public long NroDocumento { get; set; }
        public int Propuesta { get; set; }
        public string Parcela { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
    public class DtoPropuestasInhumadosList : DtoBase
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

