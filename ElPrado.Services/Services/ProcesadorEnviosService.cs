using ElPrado.Data.Interfaces;
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
            var listPendientes = await _uow.ColaEnvioFactura.BuscarPendientesAsync();

            foreach (var factura in listPendientes)
            {
                try
                {
                    await _uow.ColaEnvioFactura.MarcarEnviandoAsync(factura.CodColaEnvio);

                    await _email.EnviarFacturaAsync(factura);
                    await Task.Delay(2000);

                    await _uow.ColaEnvioFactura.MarcarEnviadoAsync(factura.CodColaEnvio);
                }
                catch (Exception ex)
                {
                    await _uow.ColaEnvioFactura.MarcarErrorAsync(factura.CodColaEnvio, ex.Message);
                    throw;
                }
            }
        }
    }
}
