using ElPrado.Core;
using ElPrado.Data.Interfaces;

namespace ElPrado.Services.Services
{
    public class ServiceBase : IDisposable
    {
        private bool disposed = false;
        private int _codLogUsuario;

        protected readonly IUnitOfWork _uow;
        protected readonly IUserContextService _userContext;

        public ServiceBase(IUnitOfWork unitOfWork, IUserContextService userContext)
        {   
            _uow = unitOfWork;
            _userContext = userContext;
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
                DisposeRecursos();
            }
            disposed = true;
        }

        protected virtual void DisposeRecursos()
        {

        }

        protected void RegistrarLogUsuario(string log, int codUsuario)
        {
            Type tipo = GetType();
            string nombreClase = tipo.Name;
            string descripcionClase = ObtenerSummaryDeClase(tipo);

            _codLogUsuario = _uow.LogsRepository.RegistrarLogUsuario(_codLogUsuario, log, codUsuario, nombreClase, descripcionClase);
        }

        protected void RegistrarLogCliente(string log, int codCliente)
        {
            Type tipo = GetType();
            string nombreClase = tipo.Name;

            _uow.LogsRepository.RegistrarLogCliente(codCliente, log, nombreClase);
        }

        protected void RegistrarLog(string log)
        {
            if (_userContext.GetCodUsuario() > 0) RegistrarLogUsuario(log, _userContext.GetCodUsuario());
            else if (_userContext.GetCodCliente() > 0) RegistrarLogCliente(log, _userContext.GetCodCliente());
        }

        private static string ObtenerSummaryDeClase(Type tipo)
        {
            // Using reflection.
            Attribute[] attrs = Attribute.GetCustomAttributes(tipo);  // Reflection.

            // Displaying output.
            foreach (Attribute attr in attrs)
            {
                if (attr is DescripcionAttribute a)
                {
                    return a.GetDescripcion();
                }
            }
            return string.Empty;
        }

    }
}
