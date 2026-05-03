using ElPrado.Core.Configuration;
using ElPrado.DataFactory.Interfaces;
using ElPrado.Dto.Dtos;
using ElPrado.Workers.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;

namespace ElPrado.Workers.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfigServicesRepository _repo;
        private readonly IMemoryCache _cache;

        public EmailService(IConfigServicesRepository configServicesRepository, IMemoryCache cache)
        {
            _repo = configServicesRepository;
            _cache = cache;
        }

        public async Task EnviarFacturaAsync(DtoColaEnvioFactura dtoFactura)
        {
            EmailSettings? settings = await _cache.GetOrCreateAsync("email_settings", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
                return await _repo.BuscarConfigMailAsync();
            });

            if (settings != null)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("El Prado - Facturacion", settings.SmtpUsuarioCobranza));
                message.To.Add(MailboxAddress.Parse(dtoFactura.MedioEnvio));
                message.Subject = "El Prado - Factura electronica";

                message.Body = new TextPart("html")
                {
                    Text = @$"<html><head><meta charset="" ISO-8859-1"" /><title>Factura Electronica</title></head>
                            <body bgcolor=""#FFFFFF"" text=""#000066"">
                            Estimado/a <b>{dtoFactura.Cliente}</b>:<br><br>
                            En el siguiente enlace encontrara la Factura Electronica:<br>
                            <a href=""{dtoFactura.UrlFactura}/{dtoFactura.IdGuid}"">{dtoFactura.NroComprobante}</a><br>
                            <br>Ante cualquier duda contactenos a los siguientes numeros:<br>
                            {dtoFactura.InformacionContacto1}<br>
                            {dtoFactura.InformacionContacto2}<br>
                            {dtoFactura.InformacionContacto3}<br>
                            Tambien puede descargar todas sus facturas desde <a href=""{dtoFactura.UrlWeb}/"">{dtoFactura.UrlWeb}</a><br><br><br>
                            <font color=""#CC0000""><b>No responda este mensaje.</b></font><br><br><br>
                            <font color=""#003300""><big><big><big><big><b>El Prado</b></big></big></big></big><br>
                            Cementerio Parque Privado</font><br>
                            </body></html>"
                };

                using var client = new SmtpClient();

                var secureOption = settings.SmtpPort == 587
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.SslOnConnect;

                await client.ConnectAsync(
                    settings.SmtpHost,
                    settings.SmtpPort,
                    secureOption);

                await client.AuthenticateAsync(
                    settings.SmtpUsuarioCobranza,
                    settings.SmtpClaveCobranza);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
