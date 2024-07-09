using ElPrado.Dto.Dtos;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
}
