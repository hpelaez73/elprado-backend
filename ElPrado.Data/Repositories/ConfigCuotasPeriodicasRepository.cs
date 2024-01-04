using ElPrado.Data.Models;

namespace ElPrado.Data.Repositories
{
    public class ConfigCuotasPeriodicasRepository : RepositoryBaseEntidad<ConfigCuotasPeriodicas>
    {
        public ConfigCuotasPeriodicasRepository(Transaccion transaccion) : base(transaccion)
        {
        }
    }
}
