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

        public void Agregar(TEntidad entidad)
        {
            _connection.Insert(entidad, _transaction);
        }

        public void Modificar(TEntidad entidad)
        {
            _connection.Update(entidad, _transaction);
        }
    }
}
