using Dapper;

namespace ElPrado.Data.Repositories
{
    public class VariosSeguridadRepository : RepositoryBase
    {
        public VariosSeguridadRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public string TraducirClave(string clave, bool esUsuario)
        {
            string sql = "SELECT T.VALOR FROM TRADUCIR_CLAVE(@clave, @esUsuario) T";
            return _connection.QuerySingle<string>(sql, new { clave, esUsuario }, _transaction);
        }
    }
}
