namespace ElPrado.Data.Models
{
    public class RefreshTokens : Entidades
    {
        [Key]
        public int CodRefreshToken { get; set; }
        public int? CodUsuario { get; set; }
        public int? CodCliente { get; set; }
        public int? CodPropuesta { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
    }
}
