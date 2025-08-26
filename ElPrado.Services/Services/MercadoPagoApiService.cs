using ElPrado.Dto.Dtos;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using MercadoPago.Resource.Preference;

namespace ElPrado.Services.Services
{
    internal class MercadoPagoApiService
    {
        public MercadoPagoApiService(string accessToken)
        {
            MercadoPagoConfig.AccessToken = accessToken;
        }

        public DtoPreferencia CrearPreferencia(List<DtoCuentasCorrientes> listCuotas, DtoClientes dtoCliente, DtoMercadoPago dtoMercadoPago, DateTime? fechaVencimiento)
        {
            // Crea el requerimiento de la preferencia
            var request = new PreferenceRequest
            {
                Items = new List<PreferenceItemRequest>(),
                NotificationUrl = dtoMercadoPago.NotificationUrl,
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = dtoMercadoPago.BackUrlsSuccess,
                    Failure = dtoMercadoPago.BackUrlsFailure,
                    Pending = dtoMercadoPago.BackUrlsPending
                },
                Payer = new()
                {
                    Name = dtoCliente.Nombre,
                    Email = dtoCliente.Email1,
                    Phone = new()
                    {
                        AreaCode = "",
                        Number = dtoCliente.Telefono1
                    },
                    Identification = new()
                    {
                        Type = dtoCliente.TipoDocumento,
                        Number = dtoCliente.NroDocumento.ToString()
                    }
                },
                PaymentMethods = new()
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
                },
                BinaryMode = true,
                AutoReturn = "approved",
                ExternalReference = dtoMercadoPago.CodPreferencia.ToString()
            };

            if (fechaVencimiento != null)
            {
                request.DateOfExpiration = fechaVencimiento.Value.AddHours(23);
            }

            foreach (DtoCuentasCorrientes item in listCuotas)
            {
                request.Items.Add(
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

            // Crea la preferencia usando el cliente
            var client = new PreferenceClient();
            Preference preference = client.Create(request);

            DtoPreferencia dtoPreferencia = new()
            {
                Id = preference.Id,
                InitPoint = preference.InitPoint
            };
            return dtoPreferencia;
        }

        public DtoPayment BuscarPago(long id)
        {
            DtoPayment dtoPayment = new();

            PaymentClient client = new();
            Payment pago = client.Get(id);

            dtoPayment.PagoAprobado = pago.Status != null && pago.Status != PaymentStatus.Rejected;
            if (pago.ExternalReference == null)
            {
                pago.ExternalReference = "Venta presencial";
            }
            else if (dtoPayment.PagoAprobado)
            {
                if (int.TryParse(pago.ExternalReference, out int codPreferencia)) dtoPayment.CodPreferencia = codPreferencia;
                else dtoPayment.ReferenciaExterna = pago.ExternalReference;

                dtoPayment.FechaPago = pago.DateApproved ?? DateTime.Today;
            }
            return dtoPayment;
        }
    }
}
