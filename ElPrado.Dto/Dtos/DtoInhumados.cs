namespace ElPrado.Dto.Dtos
{
    public class DtoInhumados
    {
    }

    public class DtoInhumadosPropuestas
    {
        public int CodDetInhumado { get; set; }
        public string NombreInhumado { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public int? NroDocumento { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public DateTime? FechaFallecimiento { get; set; }
        public DateTime FechaInhumacion { get; set; }
        public DateTime? FechaExhumacion { get; set; }
        public string NombreBIM { get; set; } = string.Empty;
        public int? NroDeclaracionJurada { get; set; }
    }

    public class DtoInhumacion
    {
        public int CodDetInhumado { get; set; }
        public string NombreInhumado { get; set; } = string.Empty;
        public int NroDeclaracionJurada { get; set; }
        public bool EsNn { get; set; }
        public bool EsSinDocumento { get; set; }
        public int? CodInhumado { get; set; }
        public int CodParcela { get; set; }
        public string Parcela { get; set; } = string.Empty;
        public string Manzana { get; set; } = string.Empty;
        public int Propuesta { get; set; }
        public string Receptaculo { get; set; } = string.Empty;
        public int Nivel { get; set; }
        public int Lugar { get; set; }
        public string Linea { get; set; } = string.Empty;
        public string Referencia1 { get; set; } = string.Empty;
        public string Referencia2 { get; set; } = string.Empty;
        public string ObservacionesInhumacion { get; set; } = string.Empty;
        public DateTime? FechaInhumacion { get; set; }
        public string TipoServicio { get; set; } = string.Empty;
        public bool ConServicioReligioso { get; set; }
        public bool ConServicioSepelio { get; set; }
        public string ProcedenciaInhumado { get; set; } = string.Empty;
        public string CatEntidadTraslado { get; set; } = string.Empty;
        public string EntidadTraslado { get; set; } = string.Empty;
        public string NombreAutInhumacion { get; set; } = string.Empty;
        public string TelefonoAutInhumacion { get; set; } = string.Empty;
        public string TipoDocumentoAutInhumacion { get; set; } = string.Empty;
        public int? NroDocumentoAutInhumacion { get; set; }
        public string ParentescoAutInhumacion { get; set; } = string.Empty;
        public string NombreBin { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public DateTime? FechaFallecimiento { get; set; }
        public int? Edad { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public int? NroDocumento { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string Nacionalidad { get; set; } = string.Empty;
        public string EstadoCivil { get; set; } = string.Empty;
        public string Profesion { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
        public string CausaFallecimiento { get; set; } = string.Empty;
        public string MedicoActuante { get; set; } = string.Empty;
        public string NroMatriculaMedico { get; set; } = string.Empty;
        public string ActaDefuncionNro { get; set; } = string.Empty;
        public string ActaDefuncionTomo { get; set; } = string.Empty;
        public string ActaDefuncionSerie { get; set; } = string.Empty;
        public string ActaDefuncionAa { get; set; } = string.Empty;
        public DateTime? FechaExhumacion { get; set; }
        public string CatEntidadDestino { get; set; } = string.Empty;
        public string EntidadDestino { get; set; } = string.Empty;
        public string NombreAutExhumacion { get; set; } = string.Empty;
        public string TelefonoAutExhumacion { get; set; } = string.Empty;
        public string TipoDocumentoAutExhumacion { get; set; } = string.Empty;
        public int? NroDocumentoAutExhumacion { get; set; }
        public string ParentescoAutExhumacion { get; set; } = string.Empty;
        public string ObservacionesExhumacion { get; set; } = string.Empty;
    }
}
