using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;

namespace ElPrado.Services.Services
{
    public class MercadoPagoService : ServiceBase
    {
        public MercadoPagoService(Transaccion? transaccion) : base(transaccion)
        {
        }

        public Resultados<string> ArmarPago(List<DtoCuentasCorrientes> listCuotas, DateTime? fechaVencimiento)
        {
            Resultados<string> resultado = new();

            ConfiguracionGeneralRepository configuracionGeneralRepository = new(transaccion);
            ClientesRepository clientesRepository = new(transaccion);

            Clientes? cliente = clientesRepository.Buscar(ConfiguracionGeneralSesion.CodCliente);
            if (cliente == null)
            {
                resultado.Agregar("El cliente no existe");
                return resultado;
            }

            // Agrega credenciales
            if (string.IsNullOrEmpty(MercadoPagoConfig.AccessToken))
            {
                MercadoPagoConfig.AccessToken = configuracionGeneralRepository.BuscarMercadoPagoAccessToken();
                if (string.IsNullOrEmpty(MercadoPagoConfig.AccessToken)) resultado.Agregar("Falta configurar el acceso a Mercado Pago");
            }

            string externalReference = string.Empty;
            PreferenceRequest preferenceReq = new()
            {
                Items = new List<PreferenceItemRequest>(),
                NotificationUrl = configuracionGeneralRepository.BuscarMercadoPagoNotificationUrl()
            };
            if (string.IsNullOrEmpty(preferenceReq.NotificationUrl)) resultado.Agregar("Falta configurar la Url de notificación de Mercado Pago");

            if (resultado.HayError) return resultado;

            foreach (DtoCuentasCorrientes item in listCuotas)
            {
                externalReference += item.Propuesta + "|" + item.Tipo + "|" + item.Codigo.ToString() + "|" + item.Cuota.ToString() + "|" + item.Pago.ToString() + "!!";
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
            externalReference += "**" + cliente.CodCliente.ToString();

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
            preferenceReq.ExternalReference = externalReference;
            preferenceReq.BackUrls = new()
            {
                Success = configuracionGeneralRepository.BuscarMercadoPagoBackUrlsSuccess(),
                Failure = configuracionGeneralRepository.BuscarMercadoPagoBackUrlsFailure(),
                Pending = configuracionGeneralRepository.BuscarMercadoPagoBackUrlsPending()
            };

            if (fechaVencimiento != null)
                preferenceReq.DateOfExpiration = fechaVencimiento;

            // Create the preference using the client
            PreferenceClient client = new();
            Preference preference = client.Create(preferenceReq);

            resultado.Valor = preference.InitPoint;

            return resultado;
        }

    }
}
