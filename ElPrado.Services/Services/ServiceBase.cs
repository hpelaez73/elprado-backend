using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class ServiceBase : IDisposable
    {
        private bool disposed = false;
        protected RepositoryBase repository;

        private readonly LogsService? logsService;

        private readonly Transaccion transaccion;
        protected bool transaccionPropia;
        protected Transaccion Transaccion { get => transaccion; }

        public ServiceBase(Transaccion? transaccion)
        {
            transaccionPropia = transaccion == null;
            this.transaccion = transaccion ?? new();
            repository = CrearRepositorio();
            if (this is not LogsService)
            {
                logsService = new(this.transaccion, this);
            }
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

        protected void RegistrarLogUsuario(string log, int codUsuario)
        {
            logsService?.RegistrarUsuario(log, codUsuario);
        }

        protected void RegistrarLogCliente(string log, int codCliente)
        {
            logsService?.RegistrarCliente(log, codCliente);
        }

        protected void RegistrarLog(string log)
        {
            if (ConfiguracionGeneralSesion.CodUsuario > 0) RegistrarLogUsuario(log, ConfiguracionGeneralSesion.CodUsuario);
            else if (ConfiguracionGeneralSesion.CodCliente > 0) RegistrarLogCliente(log, ConfiguracionGeneralSesion.CodCliente);
        }
    }
}
