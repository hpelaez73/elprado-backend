using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class MercadoPagoService : ServiceBase
    {
        public MercadoPagoService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        internal Resultados<string> ArmarPago(List<DtoCuentasCorrientes> listCuotas, int codCliente, DateTime? fechaVencimiento)
        {
            Resultados<string> resultado = new();

            bool esTesting = _userContext.EsTesting();

            DtoClientes? dtoCliente = _uow.Clientes.Visualizar(codCliente);
            if (dtoCliente == null)
            {
                resultado.Agregar("El cliente no existe");
                return resultado;
            }

            DtoMercadoPago dtoMercadoPago = new()
            {
                CodPreferencia = _uow.MercadoPago.ProximoCodigo("PREFERENCIAS_MERCADOPAGO"),
                AccessToken = _uow.ConfiguracionGeneral.BuscarMercadoPagoAccessToken(),
                NotificationUrl = _uow.ConfiguracionGeneral.BuscarMercadoPagoNotificationUrl(),
                BackUrlsSuccess = _uow.ConfiguracionGeneral.BuscarMercadoPagoBackUrlsSuccess(),
                BackUrlsFailure = _uow.ConfiguracionGeneral.BuscarMercadoPagoBackUrlsFailure(),
                BackUrlsPending = _uow.ConfiguracionGeneral.BuscarMercadoPagoBackUrlsPending()
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

            _uow.MercadoPago.Agregar(preferenciasMercadopago);
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
                    _uow.MercadoPago.Agregar(detPreferenciasMercadopagoCr);
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
                    _uow.MercadoPago.Agregar(detPreferenciasMercadopagoCp);
                }
            }
            resultado.Valor = dtoPreferencia.InitPoint;

            return resultado;
        }

        internal bool ImputarPago(long id)
        {
            string accessToken = _uow.ConfiguracionGeneral.BuscarMercadoPagoAccessToken();
            if (string.IsNullOrEmpty(accessToken)) return false;

            if (id == 123456)
            {
                // Caso de prueba de la pagina de MP
                return true;
            }

            MercadoPagoApiService mpService = new(accessToken);
            DtoPayment dtoPayment = mpService.BuscarPago(id);

            if (dtoPayment.ReferenciaExterna == "Venta presencial")
            {
                // Si es una venta presencial, no se hace nada
                return true;
            }
            else if (string.IsNullOrEmpty(dtoPayment.ReferenciaExterna))
            {
                // No se puede imputar el pago sin referencia externa
                // El pago ingreso por un medio presencial
                return true;
            }

            if (dtoPayment.PagoAprobado)
            {
                _uow.MercadoPago.ImputarPago(id, dtoPayment.CodPreferencia, dtoPayment.ReferenciaExterna == dtoPayment.CodPreferencia.ToString() ? "" : dtoPayment.ReferenciaExterna, dtoPayment.FechaPago);
                return true;
            }
            return false;
        }
    }
}
