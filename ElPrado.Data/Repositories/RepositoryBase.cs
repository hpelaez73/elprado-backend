using Dapper;
using ElPrado.Data.Models;
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
    }
}
