using ElPrado.Data;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class PropuestasService : ServiceBase
    {
        private PropuestasRepository propuestasRepository => (repository as PropuestasRepository)!;

        public PropuestasService(Transaccion? transaccion) : base(transaccion)
        {
        }

        protected override RepositoryBase CrearRepositorio()
        {
            return new PropuestasRepository(Transaccion);
        }

        public ApiResponseListado<IEnumerable<dynamic>> ListadoTitulares(DtoOpcionesListados? opcionesListado)
        {
            return propuestasRepository.ListadoTitulares(opcionesListado);
        }
    }
}
