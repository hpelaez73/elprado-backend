using ElPrado.Core.Configuration;

namespace ElPrado.DataFactory.Interfaces
{
    public interface IConfigServicesRepository
    {
        Task<EmailSettings?> BuscarConfigMailAsync();
    }
}