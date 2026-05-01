using ElPrado.Services;

namespace ElPrado.Workers
{
    /// <summary>
    /// Implementación de IUserContextService para workers (sin HttpContext)
    /// Devuelve valores por defecto ya que los workers no tienen contexto de usuario
    /// </summary>
    public class WorkerUserContextService : IUserContextService
    {
        public int GetCodUsuario()
        {
            // Workers no tienen usuario autenticado, devuelve 0 (sistema)
            return 0;
        }

        public int GetCodCliente()
        {
            // Sin contexto de cliente
            return 0;
        }

        public int GetCodPropuesta()
        {
            // Sin contexto de propuesta
            return 0;
        }

        public bool EsTesting()
        {
            return false;
        }
    }
}
