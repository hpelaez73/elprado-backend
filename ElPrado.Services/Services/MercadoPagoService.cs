using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class MercadoPagoService : ServiceBase
    {
        private MercadoPagoRepository mercadoPagoRepository => (repository as MercadoPagoRepository)!;

        public MercadoPagoService(Transaccion? transaccion) : base(transaccion)
        {
        }

        protected override RepositoryBase CrearRepositorio()
        {
            return new MercadoPagoRepository(Transaccion);
        }

        internal Resultados<string> ArmarPago(List<DtoCuentasCorrientes> listCuotas, int codCliente, DateTime? fechaVencimiento)
        {
            Resultados<string> resultado = new();

            ConfiguracionGeneralRepository configuracionGeneralRepository = new(Transaccion);
            ClientesRepository clientesRepository = new(Transaccion);

            bool esTesting = ConfiguracionGeneralSesion.StrConexion.Contains("elprado_dev");

            DtoClientes? dtoCliente = clientesRepository.Visualizar(codCliente);
            if (dtoCliente == null)
            {
                resultado.Agregar("El cliente no existe");
                return resultado;
            }

            DtoMercadoPago dtoMercadoPago = new()
            {
                CodPreferencia = mercadoPagoRepository.ProximoCodigo("PREFERENCIAS_MERCADOPAGO"),
                AccessToken = configuracionGeneralRepository.BuscarMercadoPagoAccessToken(),
                NotificationUrl = configuracionGeneralRepository.BuscarMercadoPagoNotificationUrl(),
                BackUrlsSuccess = configuracionGeneralRepository.BuscarMercadoPagoBackUrlsSuccess(),
                BackUrlsFailure = configuracionGeneralRepository.BuscarMercadoPagoBackUrlsFailure(),
                BackUrlsPending = configuracionGeneralRepository.BuscarMercadoPagoBackUrlsPending()
            };
            if (string.IsNullOrEmpty(dtoMercadoPago.AccessToken)) resultado.Agregar("Falta configurar el acceso a Mercado Pago");
            if (string.IsNullOrEmpty(dtoMercadoPago.NotificationUrl)) resultado.Agregar("Falta configurar la Url de notificación de Mercado Pago");

            if (resultado.HayError) return resultado;

            DtoPreferencia dtoPreferencia = new();
            if (esTesting)
            {
                dtoPreferencia.Id = DateTime.Now.ToString();
                dtoPreferencia.InitPoint = "https://www.mercadopago.com.ar/checkout/v1/redirect?pref_id=320850108-799d6a20-4a9c-49d1-91d2-7f953ed8a113";
            }
            else
            {
                MercadoPagoApiService mpService = new(dtoMercadoPago.AccessToken);
                dtoPreferencia = mpService.CrearPreferencia(listCuotas, dtoCliente, dtoMercadoPago, fechaVencimiento);
            }

            PreferenciasMercadopago preferenciasMercadopago = new()
            {
                CodPreferenciaMercadopago = dtoMercadoPago.CodPreferencia,
                IdPreferencia = dtoPreferencia.Id,
                CodCliente = codCliente,
                FechaCreacion = DateTime.Now,
                FechaVencimiento = fechaVencimiento
            };

            mercadoPagoRepository.Agregar(preferenciasMercadopago);
            foreach (DtoCuentasCorrientes dtoCuenta in listCuotas)
            {
                if (dtoCuenta.Tipo == "CR")
                {
                    DetPreferenciasMercadopagoCr detPreferenciasMercadopagoCr = new()
                    {
                        CodPreferenciaMercadopago = dtoMercadoPago.CodPreferencia,
                        CodCredito = dtoCuenta.Codigo,
                        Cuota = dtoCuenta.Cuota,
                        Pago = dtoCuenta.Pago,
                        Importe = dtoCuenta.Total
                    };
                    mercadoPagoRepository.Agregar(detPreferenciasMercadopagoCr);
                }
                else
                {
                    DetPreferenciasMercadopagoCp detPreferenciasMercadopagoCp = new()
                    {
                        CodPreferenciaMercadopago = dtoMercadoPago.CodPreferencia,
                        CodConfiguracion = dtoCuenta.Codigo,
                        Cuota = dtoCuenta.Cuota,
                        Pago = dtoCuenta.Pago,
                        Importe = dtoCuenta.Total
                    };
                    mercadoPagoRepository.Agregar(detPreferenciasMercadopagoCp);
                }
            }
            resultado.Valor = dtoPreferencia.InitPoint;

            return resultado;
        }

        internal bool ImputarPago(long id)
        {
            ConfiguracionGeneralRepository configuracionGeneralRepository = new(Transaccion);
            string accessToken = configuracionGeneralRepository.BuscarMercadoPagoAccessToken();
            if (string.IsNullOrEmpty(accessToken)) return false;

            if (id == 123456)
            {
                // Caso de prueba de la pagina de MP
                return true;
            }

            MercadoPagoApiService mpService = new(accessToken);
            DtoPayment dtoPayment = mpService.BuscarPago(id);

            if (dtoPayment.PagoAprobado)
            {
                mercadoPagoRepository.ImputarPago(id, dtoPayment.CodPreferencia, dtoPayment.ReferenciaExterna, dtoPayment.FechaPago);
                return true;
            }
            return false;
        }
    }
}
