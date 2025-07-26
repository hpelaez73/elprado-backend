using ElPrado.Data;
using ElPrado.Data.Repositories;
using ElPrado.Dto;

namespace ElPrado.Services.Services
{
    public class MovimientosFondosService : ServiceBase
    {
        private MovimientosFondosRepository movimientosFondosRepository => (repository as MovimientosFondosRepository)!;

        public MovimientosFondosService(Transaccion? transaccion) : base(transaccion)
        {
        }

        protected override RepositoryBase CrearRepositorio()
        {
            return new MovimientosFondosRepository(Transaccion);
        }

        public DtoMovimientosFondos? Visualizar(int codMovimientoFondo)
        {
            return movimientosFondosRepository.Visualizar(codMovimientoFondo);
        }
    }
}
