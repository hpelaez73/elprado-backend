namespace ElPrado.Dto.Dtos
{
    public class DtoLoginUsuario
    {
        public string Alias { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
    }

    public class DtoLoginCliente
    {
        public int Propuesta { get; set; }
        public long DniCuit { get; set; }
        public string Clave { get; set; } = string.Empty;
    }

    public class DtoRefreshToken
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class DtoLoginClienteAlta
    {
        public int Propuesta { get; set; }
        public long DniCuit { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
        public string ClaveConfirmacion { get; set; } = string.Empty;
    }

    public class DtoLogin
    {
        public int CodUsuario{ get; set; }
        public int CodCliente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int CodPropuesta { get; set; }
        public int Propuesta { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool EsAdmin { get; set; }
    }
}
