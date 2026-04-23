using ElPrado.Data.Interfaces;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class InhumadosService : ServiceBase
    {
        public InhumadosService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        public DtoInhumacion? BuscarInhumacion(int id)
        {
            return _uow.Inhumados.BuscarInhumacion(id);
        }

        public List<DtoServiciosInhumacionesResp> ServiciosEnCurso()
        {
            return _uow.Inhumados.ServiciosEnCurso();
        }
    }
}
