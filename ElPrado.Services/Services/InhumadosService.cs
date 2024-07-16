using ElPrado.Data;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class InhumadosService : ServiceBase
    {
        private InhumadosRepository inhumadosRepository => (repository as InhumadosRepository)!;

        public InhumadosService(Transaccion? transaccion) : base(transaccion)
        {
        }

        protected override RepositoryBase CrearRepositorio()
        {
            return new InhumadosRepository(Transaccion);
        }

        public DtoInhumacion BuscarInhumacion(int id)
        {
            return inhumadosRepository.BuscarInhumacion(id);
        }
    }
}
