namespace ElPrado.Services.Interfaces
{
    public interface IEmailService
    {
        Task EnviarFacturaAsync(string destino, string linkFactura);
    }
}
