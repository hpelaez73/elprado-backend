using ElPrado.Core.Configuration;
using ElPrado.DataFactory.Interfaces;
using ElPrado.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ElPrado.Services.Services
{
    public sealed class RecuperacionClaveEmailService : IRecuperacionClaveEmailService
    {
        private readonly IConfigServicesRepository _configServices;
        private readonly string _passwordResetUrl;

        public RecuperacionClaveEmailService(IOptions<FrontendSettings> options, IConfigServicesRepository configServices)
        {
            _passwordResetUrl = options.Value.PasswordResetUrl;
            _configServices = configServices;
        }

        public Task EnviarEnlaceAsync(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(_passwordResetUrl))
            {
                return Task.CompletedTask;
            }

            string separador = _passwordResetUrl.Contains('?') ? "&" : "?";
            string enlace = $"{_passwordResetUrl}{separador}token={Uri.EscapeDataString(token)}";
            return EnviarSeguroAsync(email, "El Prado - Recuperación de clave",
                $"<p>Solicitaste restablecer tu clave de acceso.</p><p><a href=\"{enlace}\">Restablecer clave</a></p><p>El enlace vence en 30 minutos. Si no realizaste esta solicitud, podés ignorar este correo.</p>");
        }

        public Task EnviarAvisoAsync(string email) => EnviarSeguroAsync(email,
            "El Prado - Clave restablecida",
            "<p>Tu clave de acceso fue restablecida correctamente.</p><p>Si no realizaste esta operación, contactate con El Prado.</p>");

        private async Task EnviarSeguroAsync(string email, string asunto, string cuerpo)
        {
            EmailSettings? settings = await _configServices.BuscarConfigMailAsync();
            if (settings != null)
            {
                MimeMessage message = new();
                message.From.Add(new MailboxAddress("El Prado", settings.SmtpUsuario));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = asunto;
                message.Body = new TextPart("html") { Text = cuerpo };

                using SmtpClient client = new();
                SecureSocketOptions seguridad = settings.SmtpPort == 587 ? SecureSocketOptions.StartTls : SecureSocketOptions.SslOnConnect;
                await client.ConnectAsync(settings.SmtpHost, settings.SmtpPort, seguridad);
                if (settings.SmtpAutenticacion) await client.AuthenticateAsync(settings.SmtpUsuario, settings.SmtpClave);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
