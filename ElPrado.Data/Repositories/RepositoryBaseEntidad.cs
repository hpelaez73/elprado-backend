using ElPrado.Data.Models;

namespace ElPrado.Data.Repositories
{
    public class RepositoryBaseEntidad<TEntidad> : RepositoryBase
        where TEntidad : Entidades
    {
        public RepositoryBaseEntidad(Transaccion transaccion) : base(transaccion)
        {
        }

        public TEntidad Buscar(int id)
        {
            return conexion.Get<TEntidad>(id, transaccion);
        }

        public void Modificar(TEntidad entidad)
        {
            conexion.Update(entidad, transaccion);
        }
    }
}
