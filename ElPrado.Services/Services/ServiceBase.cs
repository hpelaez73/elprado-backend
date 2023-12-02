using ElPrado.Data;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class ServiceBase : IDisposable
    {
        private bool disposed = false;
        protected RepositoryBase repository;

        private Transaccion transaccion;
        protected bool transaccionPropia;
        protected Transaccion Transaccion { get => transaccion; }

        public ServiceBase(Transaccion? transaccion)
        {
            transaccionPropia = transaccion == null;
            this.transaccion = transaccion ?? new();
            repository = CrearRepositorio();
        }

        protected virtual RepositoryBase CrearRepositorio()
        {
            return new(Transaccion);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                transaccion.Dispose();
                DisposeRecursos();
            }
            disposed = true;
        }

        protected virtual void DisposeRecursos()
        {

        }

        protected void Commit()
        {
            if (transaccionPropia) transaccion.Commit();
        }

        protected void Rollback()
        {
            if (transaccionPropia) transaccion.Rollback();
        }

        public IEnumerable<dynamic> Listado(DtoOpcionesListados? opcionesListado)
        {
            return repository.Listado(opcionesListado);
        }
    }
}
