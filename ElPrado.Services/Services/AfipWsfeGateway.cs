using ElPrado.Dto.Dtos;
using ElPrado.Services.Interfaces;
using System.Text.Json;

namespace ElPrado.Services.Services
{
    public class AfipWsfeGateway : IAfipWsfeGateway
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;

        public AfipWsfeGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<ApiResponse<DtoAfipWsfe>> ConsultarEstadoServicioAsync()
        {
            return SendAsync(HttpMethod.Get, "api/wsfe/estado-servicio");
        }

        public Task<ApiResponse<DtoAfipWsfe>> ConsultarUltimoComprobanteAsync(int codTipoComprobante, int codTalonario)
        {
            return SendAsync(HttpMethod.Get, $"api/wsfe/ultimo-comprobante/{codTipoComprobante}/{codTalonario}");
        }

        public Task<ApiResponse<DtoAfipWsfeConsultaDetalle>> ConsultarComprobanteAsync(int codTalonario, string nroComprobante)
        {
            string comprobante = Uri.EscapeDataString(nroComprobante);
            return SendAsyncDetalle(HttpMethod.Get, $"api/wsfe/comprobante/{codTalonario}/{comprobante}");
        }

        public Task<ApiResponse<DtoAfipWsfe>> ActualizarCAEAsync(int codTalonario, string nroComprobante)
        {
            string comprobante = Uri.EscapeDataString(nroComprobante);
            return SendAsync(HttpMethod.Post, $"api/wsfe/actualizar-cae/{codTalonario}/{comprobante}");
        }

        public Task<ApiResponse<DtoAfipWsfe>> SolicitarCAEAsync(int codTalonario, string nroComprobante)
        {
            string comprobante = Uri.EscapeDataString(nroComprobante);
            return SendAsync(HttpMethod.Post, $"api/wsfe/solicitar-cae/{codTalonario}/{comprobante}");
        }

        private async Task<ApiResponse<DtoAfipWsfe>> SendAsync(HttpMethod method, string requestUri)
        {
            ApiResponse<DtoAfipWsfe> apiResponse = new();
            try
            {
                using HttpRequestMessage request = new(method, requestUri);
                using HttpResponseMessage response = await _httpClient.SendAsync(request);
                string content = await response.Content.ReadAsStringAsync();

                DtoAfipWsfeResponse? afipResponse = JsonSerializer.Deserialize<DtoAfipWsfeResponse>(content, JsonOptions);
                if (afipResponse == null)
                {
                    apiResponse.Agregar("Afip.WebApi devolvio una respuesta vacia o invalida");
                    return apiResponse;
                }

                apiResponse.Success = afipResponse.Success;
                apiResponse.Message = afipResponse.Message;
                apiResponse.Data = new DtoAfipWsfe
                {
                    CodTalonario = afipResponse.CodTalonario,
                    NroComprobante = afipResponse.NroComprobante,
                    NroCAE = afipResponse.NroCAE
                };
                return apiResponse;
            }
            catch (HttpRequestException)
            {
                apiResponse.Agregar("No se pudo conectar con Afip.WebApi.");
                return apiResponse;
            }
            catch (TaskCanceledException)
            {
                apiResponse.Agregar("Afip.WebApi no respondio dentro del tiempo esperado.");
                return apiResponse;
            }
            catch (JsonException)
            {
                apiResponse.Agregar("Afip.WebApi devolvio una respuesta invalida.");
                return apiResponse;
            }
        }

        private async Task<ApiResponse<DtoAfipWsfeConsultaDetalle>> SendAsyncDetalle(HttpMethod method, string requestUri)
        {
            ApiResponse<DtoAfipWsfeConsultaDetalle> apiResponse = new();
            try
            {
                using HttpRequestMessage request = new(method, requestUri);
                using HttpResponseMessage response = await _httpClient.SendAsync(request);
                string content = await response.Content.ReadAsStringAsync();

                ApiResponse<DtoAfipWsfeConsultaDetalle>? afipResponse = JsonSerializer.Deserialize<ApiResponse<DtoAfipWsfeConsultaDetalle>>(content, JsonOptions);
                if (afipResponse == null)
                {
                    apiResponse.Agregar("Afip.WebApi devolvio una respuesta vacia o invalida");
                    return apiResponse;
                }

                return afipResponse;
            }
            catch (HttpRequestException)
            {
                apiResponse.Agregar("No se pudo conectar con Afip.WebApi.");
                return apiResponse;
            }
            catch (TaskCanceledException)
            {
                apiResponse.Agregar("Afip.WebApi no respondio dentro del tiempo esperado.");
                return apiResponse;
            }
            catch (JsonException)
            {
                apiResponse.Agregar("Afip.WebApi devolvio una respuesta invalida.");
                return apiResponse;
            }
        }
    }
}
