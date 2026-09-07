using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Interfaces
{
    public interface IAfipWsfeGateway
    {
        Task<ApiResponse<DtoAfipWsfe>> ConsultarEstadoServicioAsync();
        Task<ApiResponse<DtoAfipWsfe>> ConsultarUltimoComprobanteAsync(int codTipoComprobante, int codTalonario);
        Task<ApiResponse<DtoAfipWsfeConsultaDetalle>> ConsultarComprobanteAsync(int codTalonario, string nroComprobante);
        Task<ApiResponse<DtoAfipWsfe>> ActualizarCAEAsync(int codTalonario, string nroComprobante);
        Task<ApiResponse<DtoAfipWsfe>> SolicitarCAEAsync(int codTalonario, string nroComprobante);
    }
}
