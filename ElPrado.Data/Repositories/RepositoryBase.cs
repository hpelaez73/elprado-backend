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
    }
}
