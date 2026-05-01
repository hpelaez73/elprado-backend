using ElPrado.Core.Configuration;

namespace ElPrado.Data.Interfaces
{
    public interface IConfigServicesRepository
    {
        Task<EmailSettings?> BuscarConfigMailAsync();
    }
}