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
}
