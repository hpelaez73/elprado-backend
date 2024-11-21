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
            return connection.Get<TEntidad>(id, transaction);
        }

        public void Agregar(TEntidad entidad)
        {
            connection.Insert(entidad, transaction);
        }

        public void Modificar(TEntidad entidad)
        {
            connection.Update(entidad, transaction);
        }
    }
}
