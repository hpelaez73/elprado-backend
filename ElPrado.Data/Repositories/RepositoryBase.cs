using System.Data;

namespace ElPrado.Data.Repositories
{
    public class RepositoryBase
    {
        protected bool transaccionPropia = true;
        protected IDbConnection conexion;
        protected IDbTransaction transaccion;

        public RepositoryBase(Transaccion transaccion)
        {
            conexion = transaccion.GetConnection();
            this.transaccion = transaccion.GetTransaction();
        }
    }
}
