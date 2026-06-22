using Afip.Data.Models;

namespace Afip.Data.Interfaces
{
    public interface IAfipRepository
    {
        // Métodos relacionados con la configuración de AFIP
        Task<AfipConfiguraciones?> GetAfipConfigurationAsync();
        Task UpdateAfipConfigurationAsync(AfipConfiguraciones config);

        // Métodos relacionados con los comprobantes
        Task<AfipComprobante?> GetComprobanteParaCAEAsync(int codTalonario, string nroComprobante);
        Task<IEnumerable<AfipAlicuotas>> GetAlicuotasComprobanteAsync(int codTalonario, string nroComprobante);
        Task ActualizarRespuestaCAEAsync(int codTalonario, string nroComprobante, DateTime afipFechaProceso, string afipResultado, string afipCae, DateTime? afipVencimientoCae, string? afipObservaciones, string? afipJson);

        Task<AfipTiposComprobantes?> GetAfipTipoComprobanteAsync(int codTalonario, int codTipoComprobante);
        Task ActualizarUltimoComprobanteEmitidoAsync(AfipUltimoComprobanteEmitido ultimoComprobante);

        // Metodos relacionados con errores en la obtención del CAE
        Task ActualizarErrorCAEAsync(int codTalonario, string nroComprobante, DateTime afipFechaProceso, string afipResultado, string msgError, string? afipJson);
        Task InsertarErrorAsync(AfipErrores error);
    }
}