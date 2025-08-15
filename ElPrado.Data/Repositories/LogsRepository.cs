
using Dapper;

namespace ElPrado.Data.Repositories
{
    public class LogsRepository : RepositoryBase
    {
        public LogsRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public int RegistrarLogUsuario(int codLogUsuario, string log, int codUsuario, string nombreClase, string descripcionClase)
        {
            string sql = "SELECT L.COD_LOG_USUARIO FROM REGISTRAR_LOG_USUARIO(@codLogUsuario, @log, @codUsuario, @nombreClase, @descripcionClase) L";
            return _connection.QuerySingleOrDefault<int>(sql, new { codLogUsuario, log, codUsuario, nombreClase, descripcionClase }, _transaction);
        }

        public void RegistrarLogCliente(int codCliente, string log, string nombreClase)
        {
            string sql = "EXECUTE PROCEDURE REGISTRAR_LOG_CLIENTE(@codCliente, @nombreClase, @log)";
            _connection.Execute(sql, new { codCliente, log, nombreClase}, _transaction);
        }
    }
}
