using Dapper;
using ElPrado.Dto.Dtos;
using System.Data;

namespace ElPrado.Data.Repositories
{
    public class RepositoryBase
    {
        protected IDbConnection connection;
        protected IDbTransaction transaction;
        protected Transaccion transaccion;

        public RepositoryBase(Transaccion transaccion)
        {
            this.transaccion = transaccion;
            connection = transaccion.GetConnection();
            transaction = transaccion.GetTransaction();
        }

        public IEnumerable<dynamic> Listado(DtoOpcionesListados? opcionesListado)
        {
            return Enumerable.Empty<dynamic>();
        }

        public bool Existe(string nombreTabla, string nombreCampo, object valor)
        {
            string sql = $"SELECT 1 FROM RDB$DATABASE WHERE EXISTS(SELECT 1 FROM {nombreTabla.ToUnderscoreCase().ToUpper()} WHERE {nombreCampo.ToUnderscoreCase().ToUpper()} = @valor)";
            return connection.QuerySingleOrDefault<bool>(sql, new { valor }, transaction);
        }

        public int ProximoCodigo(string nombreTabla)
        {
            string sql = @"SELECT GENERADOR FROM TABLAS_SISTEMAS
                WHERE NOMBRE_TABLA = @nombreTabla
                AND NOT GENERADOR IS NULL";

            string generador = connection.QuerySingle<string>(sql, new { nombreTabla }, transaction);

            sql = $"SELECT GEN_ID({generador}, 1) AS VALOR FROM RDB$DATABASE";
            return connection.QuerySingleOrDefault<int>(sql, null, transaction);
        }
    }
}
