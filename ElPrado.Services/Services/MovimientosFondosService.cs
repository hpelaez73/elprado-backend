using ElPrado.Data;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class MovimientosFondosService : ServiceBase
    {
        public MovimientosFondosService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        public DtoMovimientosFondos? Visualizar(int codMovimientoFondo)
        {
            return _uow.MovimientosFondos.Visualizar(codMovimientoFondo);
        }
    }
}
