using Dapper;

namespace ElPrado.Data.Repositories
{
    public class VariosSeguridadRepository : RepositoryBase
    {
        public VariosSeguridadRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public string TraducirClave(string clave, bool esUsuario)
        {
            string sql = "SELECT T.VALOR FROM TRADUCIR_CLAVE(@clave, @esUsuario) T";
            return conexion.QuerySingle<string>(sql, new { clave, esUsuario }, transaccion);
        }
    }
}
