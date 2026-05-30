using Dapper;
using ElPrado.Data.Models;

namespace ElPrado.Data.Repositories
{
    public class ConfigCuotasPeriodicasRepository : RepositoryBaseEntidad<ConfigCuotasPeriodicas>
    {
        public ConfigCuotasPeriodicasRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public async Task GenerarCuotasPeriodicasAsync(int codConfiguracion, DateTime fechaHasta)
        {
            string sql = "EXECUTE PROCEDURE GENERAR_CUOTAS_PERIODICAS(@codConfiguracion, @fechaHasta)";
            await _connection.ExecuteAsync(sql, new { codConfiguracion, fechaHasta }, _transaction);
        }
    }
}
