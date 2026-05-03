using ElPrado.Dto.Dtos;

namespace ElPrado.DataFactory.Interfaces
{
    public interface IColaEnvioFacturaRepository
    {
        Task<IEnumerable<DtoColaEnvioFactura>> BuscarPendientesAsync();
        Task MarcarEnviadoAsync(int codColaEnvio);
        Task MarcarEnviandoAsync(int codColaEnvio);
        Task MarcarErrorAsync(int codColaEnvio, string error);
    }
}