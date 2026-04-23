using ElPrado.Data.Interfaces;
using ElPrado.Data.Models;
using ElPrado.Services.Interfaces;

namespace ElPrado.Services.Services
{
    public class ProcesadorEnviosService : ServiceBase
    {
        private readonly IEmailService _email;

        public ProcesadorEnviosService(IUnitOfWork unitOfWork, IUserContextService userContext, IEmailService email) : base(unitOfWork, userContext)
        {
            _email = email;
        }

        public async Task ProcesarAsync()
        {
            var pendientes = await _uow.ColaEnvioFactura.BuscarPendientesAsync(20);

            foreach (var envio in pendientes)
            {
                try
                {
                    await _uow.ColaEnvioFactura.MarcarEnviandoAsync(envio.CodColaEnvio);

                    var link = GenerarLink(envio);
                    await _email.EnviarFacturaAsync(envio.MedioEnvio, link);

                    await _uow.ColaEnvioFactura.MarcarEnviadoAsync(envio.CodColaEnvio);

                    // TODO: insertar en HIST_ENVIOS_FACTURAS
                }
                catch (Exception ex)
                {
                    await _uow.ColaEnvioFactura.MarcarErrorAsync(envio.CodColaEnvio, ex.Message);
                }
            }
        }

        private string GenerarLink(ColaEnvioFactura envio)
        {
            return $"https://tu-api/api/facturas/{envio.CodTalonario}/{envio.NroComprobante}/pdf";
        }
    }
}
