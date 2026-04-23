namespace ElPrado.Core.Configuration
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public bool SmtpAutenticacion { get; set; }
        public string SmtpUsuario { get; set; } = string.Empty;
        public string SmtpClave { get; set; } = string.Empty;
        public string SmtpUsuarioCobranza { get; set; } = string.Empty;
        public string SmtpClaveCobranza { get; set; } = string.Empty;
    }
}