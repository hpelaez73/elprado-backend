namespace ElPrado.Dto.Dtos
{
    public class DtoClientes : DtoBase
    {
        public int CodCliente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string NombreComercial { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        // Datos generales
        public string TipoDocumento { get; set; } = string.Empty;
        public long? NroDocumento { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public DateTime? FechaFallecimiento { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string EstadoCivil { get; set; } = string.Empty;
        public string Nacionalidad { get; set; } = string.Empty;
        public string Profesion { get; set; } = string.Empty;
        public bool EsPersonaFisica { get; set; }
        // Datos de ventas
        public string CategoriaIva { get; set; } = string.Empty;
        public long? Cuit { get; set; }
        public string IngresosBrutos { get; set; } = string.Empty;
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }
        public bool PermitirFacturarReintegros { get; set; }
        // Datos de contactos
        public string Telefono1 { get; set; } = string.Empty;
        public string Telefono2 { get; set; } = string.Empty;
        public string Telefono3 { get; set; } = string.Empty;
        public string ReferenciaTelefono1 { get; set; } = string.Empty;
        public string ReferenciaTelefono2 { get; set; } = string.Empty;
        public string ReferenciaTelefono3 { get; set; } = string.Empty;
        public bool LlamarTelefono1 { get; set; }
        public bool LlamarTelefono2 { get; set; }
        public bool LlamarTelefono3 { get; set; }
        public bool MensajeTelefono1 { get; set; }
        public bool MensajeTelefono2 { get; set; }
        public bool MensajeTelefono3 { get; set; }
        public string Email1 { get; set; } = string.Empty;
        public string Email2 { get; set; } = string.Empty;
        public bool NotificacionesEmail1 { get; set; }
        public bool NotificacionesEmail2 { get; set; }
        public bool FacturasEmail1 { get; set; }
        public bool FacturasEmail2 { get; set; }
        // Domicilios
        public int? CodDomicilioParticular { get; set; }
        public int? CodDomicilioLaboral { get; set; }
        public int? CodDomicilioCobranza { get; set; }
        public DtoDomicilos? DomicilioParticular { get; set; }
        public DtoDomicilos? DomicilioLaboral { get; set; }
        public DtoDomicilos? DomicilioCobranza { get; set; }
    }

    public class DtoClientesPropuestas
    {
        public int CodCliente { get; set; }
        public int Orden { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public long NroDocumento { get; set; }
        public string Telefono { get; set; } = string.Empty;
        public string TelefonoMovil { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Localidad { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public DateTime? FechaAlta { get; set; }
        public string Web { get; set; } = string.Empty;
    }

    public class DtoClientesPropuestasFacturasPagos
    {
        public int CodCliente { get; set; }
        public bool Factura { get; set; }
        public bool Pago { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public long NroDocumento { get; set; }
        public string Telefono { get; set; } = string.Empty;
        public string TelefonoMovil { get; set; } = string.Empty;
    }

    public class DtoClientesPropuestasHistorial
    {
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaAlta { get; set; }
        public string AutorizaAlta { get; set; } = string.Empty;
        public string UsuarioAlta { get; set; } = string.Empty;
        public DateTime? FechaBaja { get; set; }
        public string AutorizaBaja { get; set; } = string.Empty;
        public string UsuarioBaja { get; set; } = string.Empty;
    }
}
