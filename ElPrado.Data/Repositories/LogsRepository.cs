
using Dapper;

namespace ElPrado.Data.Repositories
{
    public class LogsRepository : RepositoryBase
    {
        public LogsRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public int RegistrarLogUsuario(int codLogUsuario, string log, int codUsuario, string nombreClase, string descripcionClase)
        {
            string sql = "SELECT L.COD_LOG_USUARIO FROM REGISTRAR_LOG_USUARIO(@codLogUsuario, @log, @codUsuario, @nombreClase, @descripcionClase) L";
            return connection.QuerySingleOrDefault<int>(sql, new { codLogUsuario, log, codUsuario, nombreClase, descripcionClase }, transaction);
        }

        public void RegistrarLogCliente(int codCliente, string log, string nombreClase)
        {
            string sql = "EXECUTE PROCEDURE REGISTRAR_LOG_CLIENTE(@codCliente, @nombreClase, @log)";
            connection.Execute(sql, new { codCliente, log, nombreClase}, transaction);
        }
    }
}
