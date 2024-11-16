using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Repositories;

namespace ElPrado.Services.Services
{
    public class LogsService : ServiceBase
    {
        private LogsRepository logsRepository => (repository as LogsRepository)!;
        private readonly ServiceBase servicioPadre;
        private int codLogUsuario;

        public LogsService(Transaccion? transaccion, ServiceBase serviceBase) : base(transaccion)
        {
            servicioPadre = serviceBase;
        }

        protected override RepositoryBase CrearRepositorio()
        {
            return new LogsRepository(Transaccion);
        }

        // quien lo llame se encarga del commit
        internal void RegistrarUsuario(string log, int codUsuario)
        {
            Type tipo = servicioPadre.GetType();
            string nombreClase = tipo.Name;
            string descripcionClase = ObtenerSummaryDeClase(tipo);

            codLogUsuario = logsRepository.RegistrarLogUsuario(codLogUsuario, log, codUsuario, nombreClase, descripcionClase);
        }

        internal void RegistrarCliente(string log, int codCliente)
        {
            Type tipo = servicioPadre.GetType();
            string nombreClase = tipo.Name;

            logsRepository.RegistrarLogCliente(codCliente, log, nombreClase);
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
