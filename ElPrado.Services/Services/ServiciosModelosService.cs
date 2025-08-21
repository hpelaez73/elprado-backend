using ElPrado.Data;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class ServiciosModelosService : ServiceBase
    {
        public ServiciosModelosService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        public List<DtoServiciosPropuesta> ServiciosPropuesta(int codPropuesta)
        {
            return _uow.ServiciosModelos.ServiciosPropuesta(codPropuesta);
        }

        public List<DtoServiciosUtilizadosPropuesta> ServiciosUtilizadosPropuesta(int codPropuesta)
        {
            return _uow.ServiciosModelos.ServiciosUtilizadosPropuesta(codPropuesta);
        }

        public List<DtoBeneficiariosPropuesta> BeneficiariosPropuesta(int codPropuesta)
        {
            return _uow.ServiciosModelos.BeneficiariosPropuesta(codPropuesta);
        }
    }
}
