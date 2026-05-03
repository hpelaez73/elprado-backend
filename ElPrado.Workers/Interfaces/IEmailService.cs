using ElPrado.Dto.Dtos;

namespace ElPrado.Workers.Interfaces
{
    public interface IEmailService
    {
        Task EnviarFacturaAsync(DtoColaEnvioFactura dtoFactura);
    }
}
