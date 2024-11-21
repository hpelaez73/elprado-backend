using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using MercadoPago.Resource.Preference;
using System.Linq;

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

            Clientes? cliente = clientesRepository.Buscar(codCliente);
            if (cliente == null)
            {
                resultado.Agregar("El cliente no existe");
                return resultado;
            }

            // Agrega credenciales
            if (!esTesting && string.IsNullOrEmpty(MercadoPagoConfig.AccessToken))
            {
                MercadoPagoConfig.AccessToken = configuracionGeneralRepository.BuscarMercadoPagoAccessToken();
                if (string.IsNullOrEmpty(MercadoPagoConfig.AccessToken)) resultado.Agregar("Falta configurar el acceso a Mercado Pago");
            }

            int codPreferencia = mercadoPagoRepository.ProximoCodigo("PREFERENCIAS_MERCADOPAGO");

            PreferenceRequest preferenceReq = new()
            {
                Items = new List<PreferenceItemRequest>(),
                NotificationUrl = configuracionGeneralRepository.BuscarMercadoPagoNotificationUrl()
            };
            if (string.IsNullOrEmpty(preferenceReq.NotificationUrl)) resultado.Agregar("Falta configurar la Url de notificación de Mercado Pago");

            if (resultado.HayError) return resultado;

            foreach (DtoCuentasCorrientes item in listCuotas)
            {
                //externalReference += item.Propuesta + "|" + item.Tipo + "|" + item.Codigo.ToString() + "|" + item.Cuota.ToString() + "|" + item.Pago.ToString() + "!!";
                preferenceReq.Items.Add(
                    new()
                    {
                        Id = item.Propuesta.ToString(),
                        Title = item.Propuesta + " - " + item.Categoria,
                        Description = "Periodo " + item.FechaCuota.ToString("MM/yyyy"),
                        Quantity = 1,
                        CurrencyId = "ARS",
                        UnitPrice = (decimal)item.Total
                    }
                );
            }
            //externalReference += "**" + cliente.CodCliente.ToString();

            preferenceReq.Payer = new()
            {
                Name = cliente.Nombre,
                Email = cliente.Email,
                Phone = new()
                {
                    AreaCode = "",
                    Number = cliente.Telefono
                },
                Identification = new()
                {
                    Type = cliente.TipoDocumento,
                    Number = cliente.NroDocumento.ToString()
                }
            };

            preferenceReq.PaymentMethods = new()
            {
                ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
                {
                    new ()
                    {
                        Id = "ticket"
                    },
                    new ()
                    {
                        Id = "atm"
                    }
                },
                Installments = 1
            };
            preferenceReq.BinaryMode = true;
            preferenceReq.AutoReturn = "approved";
            preferenceReq.ExternalReference = codPreferencia.ToString();
            preferenceReq.BackUrls = new()
            {
                Success = configuracionGeneralRepository.BuscarMercadoPagoBackUrlsSuccess(),
                Failure = configuracionGeneralRepository.BuscarMercadoPagoBackUrlsFailure(),
                Pending = configuracionGeneralRepository.BuscarMercadoPagoBackUrlsPending()
            };

            if (fechaVencimiento != null)
            {
                preferenceReq.DateOfExpiration = fechaVencimiento.Value.AddHours(23);
            }

            // Create the preference using the client
            Preference preference;
            if (esTesting)
            {
                preference = new()
                {
                    Id = DateTime.Now.ToString(),
                    InitPoint = "https://www.mercadopago.com.ar/checkout/v1/redirect?pref_id=320850108-799d6a20-4a9c-49d1-91d2-7f953ed8a113"
                };
            }
            else
            {
                PreferenceClient client = new();
                preference = client.Create(preferenceReq);
            }

            PreferenciasMercadopago preferenciasMercadopago = new()
            {
                CodPreferenciaMercadopago = codPreferencia,
                IdPreferencia = preference.Id,
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
                        CodPreferenciaMercadopago = codPreferencia,
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
                        CodPreferenciaMercadopago = codPreferencia,
                        CodConfiguracion = dtoCuenta.Codigo,
                        Cuota = dtoCuenta.Cuota,
                        Pago = dtoCuenta.Pago,
                        Importe = dtoCuenta.Total
                    };
                    mercadoPagoRepository.Agregar(detPreferenciasMercadopagoCp);
                }
            }
            resultado.Valor = preference.InitPoint;

            return resultado;
        }

        public bool ImputarPago(long id)
        {
            ConfiguracionGeneralRepository configuracionGeneralRepository = new(Transaccion);
            // Agrega credenciales
            if (string.IsNullOrEmpty(MercadoPagoConfig.AccessToken))
            {
                MercadoPagoConfig.AccessToken = configuracionGeneralRepository.BuscarMercadoPagoAccessToken();
                if (string.IsNullOrEmpty(MercadoPagoConfig.AccessToken)) return false;
            }

            if (id == 123456)
            {
                // Caso de prueba de la pagina de MP
                return true;
            }

            try
            {
                PaymentClient client = new();
                Payment pago = client.Get(id);
                if (pago.Status != null && pago.Status != PaymentStatus.Rejected)
                {
                    mercadoPagoRepository.ImputarPago(id, Convert.ToInt32(pago.ExternalReference), pago.DateApproved ?? DateTime.Today);
                    Commit();

                    return true;
                }
            }
            catch
            {
                Rollback();
                throw;
            }
            return false;
        }
    }
}
