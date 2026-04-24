using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Interfaces
{
    public interface IEmailService
    {
        Task EnviarFacturaAsync(DtoColaEnvioFactura dtoFactura);
    }
}
