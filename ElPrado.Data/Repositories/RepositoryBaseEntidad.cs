using ElPrado.Data.Models;

namespace ElPrado.Data.Repositories
{
    public class RepositoryBaseEntidad<TEntidad> : RepositoryBase
        where TEntidad : Entidades
    {
        public RepositoryBaseEntidad(DbContext dbContext) : base(dbContext)
        {
        }

        public TEntidad Buscar(int id)
        {
            return _connection.Get<TEntidad>(id, _transaction);
        }

        public int? Agregar(TEntidad entidad)
        {
            return _connection.Insert<TEntidad>(entidad, _transaction);
        }

        public void Modificar(TEntidad entidad)
        {
            _connection.Update<TEntidad>(entidad, _transaction);
        }

        public void Eliminar(int id)
        {
            _connection.Delete<TEntidad>(id, _transaction);
        }
    }
}
