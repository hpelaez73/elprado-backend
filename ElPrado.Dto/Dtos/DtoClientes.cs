namespace ElPrado.Dto.Dtos
{
    public class DtoClientes : DtoBase
    {
        public int CodCliente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public long NroDocumento { get; set; }
        public string Telefono { get; set; } = string.Empty;
        public string TelefonoMovil { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
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
