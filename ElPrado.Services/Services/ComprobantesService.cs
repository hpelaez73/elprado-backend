using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class ComprobantesService : ServiceBase
    {
        private ComprobantesRepository comprobantesRepository => (repository as ComprobantesRepository)!;

        public ComprobantesService(Transaccion? transaccion) : base(transaccion)
        {
        }

        protected override RepositoryBase CrearRepositorio()
        {
            return new ComprobantesRepository(Transaccion);
        }

        public IEnumerable<DtoComprobantesFacturasElectronicas> Facturas(int periodo)
        {
            return comprobantesRepository.Facturas(ConfiguracionGeneralSesion.CodCliente, new DateTime(periodo, 1, 1), new DateTime(periodo, 12, 31));
        }

        public List<int> PeriodosFacturacion()
        {
            DtoComprobantesPeriodo? periodo = comprobantesRepository.PeriodosFacturacion(ConfiguracionGeneralSesion.CodCliente);
            if (periodo == null)
            {
                return new();
            }
            else
            {
                List<int> listPeriodos = new();
                for (int i = periodo.MaxFecha.Year; i >= periodo.MinFecha.Year; i--)
                {
                    listPeriodos.Add(i);
                }
                return listPeriodos;
            }
        }
    }
}
