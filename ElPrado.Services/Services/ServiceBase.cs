using ElPrado.Data;

namespace ElPrado.Services.Services
{
    public class ServiceBase : IDisposable
    {
        private bool disposed = false;
        protected Transaccion transaccion;

        public ServiceBase(Transaccion? transaccion)
        {
            this.transaccion = transaccion ?? new();
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
    }
}
