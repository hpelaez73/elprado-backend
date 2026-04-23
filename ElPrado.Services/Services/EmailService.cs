using ElPrado.Core.Configuration;
using ElPrado.Data.Interfaces;
using ElPrado.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ElPrado.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IConfigServicesRepository configServicesRepository)
        {
            _emailSettings = configServicesRepository.BuscarConfigMail() ?? new EmailSettings();
        }

        public async Task EnviarFacturaAsync(string destino, string linkFactura)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Sistema", _emailSettings.SmtpUsuarioCobranza));
            message.To.Add(MailboxAddress.Parse(destino));
            message.Subject = "Factura disponible";

            message.Body = new TextPart("html")
            {
                Text = $"<p>Descargar factura:</p><a href='{linkFactura}'>Ver factura</a>"
            };

            using var client = new SmtpClient();

            var secureOption = _emailSettings.SmtpPort == 587
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.SslOnConnect;

            await client.ConnectAsync(
                _emailSettings.SmtpHost,
                _emailSettings.SmtpPort,
                secureOption);

            await client.AuthenticateAsync(
                _emailSettings.SmtpUsuarioCobranza,
                _emailSettings.SmtpClaveCobranza);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
