using ElPrado.Data.Interfaces;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class ServiciosModelosService : ServiceBase
    {
        public ServiciosModelosService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        public DtoServiciosPropuesta ServiciosPropuesta(int codPropuesta)
        {
            DtoServiciosPropuesta serviciosPropuesta = new()
            {
                ListServiciosHabilitados = _uow.ServiciosModelos.ServiciosPropuesta(codPropuesta),
                ListServiciosUtilizados = _uow.ServiciosModelos.ServiciosUtilizadosPropuesta(codPropuesta),
                ListServiciosBeneficiarios = _uow.ServiciosModelos.BeneficiariosPropuesta(codPropuesta)
            };
            return serviciosPropuesta;
        }
    }
}
