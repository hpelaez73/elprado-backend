using Dapper;
using ElPrado.Dto.Dtos;
using System.Data;

namespace ElPrado.Data.Repositories
{
    public class RepositoryBase
    {
        protected DbContext _dbContext;
        protected IDbConnection _connection;
        protected IDbTransaction _transaction;

        public RepositoryBase(DbContext dbContext)
        {
            _dbContext = dbContext;
            _connection = dbContext.GetConnection();
            _transaction = dbContext.GetTransaction();
        }

        public virtual ApiResponseListado<IEnumerable<dynamic>> Listado(DtoOpcionesListados opcionesListado)
        {
            return new ApiResponseListado<IEnumerable<dynamic>>();
        }

        public bool ExisteValor(string nombreTabla, string nombreCampo, object valor)
        {
            string sql = $"SELECT 1 FROM RDB$DATABASE WHERE EXISTS(SELECT 1 FROM {nombreTabla.ToUnderscoreCase().ToUpper()} WHERE {nombreCampo.ToUnderscoreCase().ToUpper()} = @valor)";
            return _connection.QuerySingleOrDefault<bool>(sql, new { valor }, _transaction);
        }

        public int ProximoCodigo(string nombreTabla)
        {
            string sql = @"SELECT GENERADOR FROM TABLAS_SISTEMAS
                WHERE NOMBRE_TABLA = @nombreTabla
                AND NOT GENERADOR IS NULL";

            string generador = _connection.QuerySingle<string>(sql, new { nombreTabla }, _transaction);

            sql = $"SELECT GEN_ID({generador}, 1) AS VALOR FROM RDB$DATABASE";
            return _connection.QuerySingleOrDefault<int>(sql, null, _transaction);
        }
    }
}
