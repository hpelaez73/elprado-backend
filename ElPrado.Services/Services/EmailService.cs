using ElPrado.Core.Configuration;
using ElPrado.Data.Interfaces;
using ElPrado.Dto.Dtos;
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

        public async Task EnviarFacturaAsync(DtoColaEnvioFactura dtoFactura)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("El Prado - Facturacion", _emailSettings.SmtpUsuarioCobranza));
            message.To.Add(MailboxAddress.Parse(dtoFactura.MedioEnvio));
            message.Subject = "El Prado - Factura electronica";

            message.Body = new TextPart("html")
            {
                Text = @$"  <html><head><meta charset="" ISO-8859-1"" /><title>Factura Electronica</title></head>
                            <body bgcolor=""#FFFFFF"" text=""#000066"">
                            Estimado/a <b>{ dtoFactura.Cliente }</b>:<br><br>
                            En el siguiente enlace encontrara la Factura Electronica:<br>
                            <a href=""{ dtoFactura.UrlFactura }/{ dtoFactura.IdGuid }"">{ dtoFactura.NroComprobante }</a><br>
                            <br>Ante cualquier duda contactenos a los siguientes numeros:<br>
                            { dtoFactura.InformacionContacto1 }<br>
                            { dtoFactura.InformacionContacto2 }<br>
                            { dtoFactura.InformacionContacto3 }<br>
                            Tambien puede descargar todas sus facturas desde <a href=""{ dtoFactura.UrlWeb }/"">{ dtoFactura.UrlWeb }</a><br><br><br>
                            <font color=""#CC0000""><b>No responda este mensaje.</b></font><br><br><br>
                            <font color=""#003300""><big><big><big><big><b>El Prado</b></big></big></big></big><br>
                            Cementerio Parque Privado</font><br>
                            </body></html>"
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
