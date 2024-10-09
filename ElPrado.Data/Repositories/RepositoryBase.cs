using Dapper;
using ElPrado.Dto.Dtos;
using System.Data;

namespace ElPrado.Data.Repositories
{
    public class RepositoryBase
    {
        protected IDbConnection conexion;
        protected IDbTransaction transaccion;

        public RepositoryBase(Transaccion transaccion)
        {
            conexion = transaccion.GetConnection();
            this.transaccion = transaccion.GetTransaction();
        }

        public IEnumerable<dynamic> Listado(DtoOpcionesListados? opcionesListado)
        {
            return Enumerable.Empty<dynamic>();
        }

        public bool Existe(string nombreTabla, string nombreCampo, object valor)
        {
            string sql = $"SELECT 1 FROM RDB$DATABASE WHERE EXISTS(SELECT 1 FROM {nombreTabla.ToUnderscoreCase().ToUpper()} WHERE {nombreCampo.ToUnderscoreCase().ToUpper()} = @valor)";
            return conexion.QuerySingleOrDefault<bool>(sql, new { valor }, transaccion);
        }

        public int ProximoCodigo(string nombreTabla)
        {
            string sql = @"SELECT GENERADOR FROM TABLAS_SISTEMAS
                WHERE NOMBRE_TABLA = @nombreTabla
                AND NOT GENERADOR IS NULL";

            string generador = conexion.QuerySingle<string>(sql, new { nombreTabla }, transaccion);

            sql = $"SELECT GEN_ID({generador}, 1) AS VALOR FROM RDB$DATABASE";
            return conexion.QuerySingleOrDefault<int>(sql, null, transaccion);
        }
    }
}
