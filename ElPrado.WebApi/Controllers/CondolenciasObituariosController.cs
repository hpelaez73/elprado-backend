using ElPrado.Data.Interfaces;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;

namespace ElPrado.WebApi.Controllers
{
    public class CondolenciasObituariosController : ControladorBaseCrud<CondolenciasObituarios, DtoCondolenciasObituarios>
    {
        public CondolenciasObituariosController(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override ServiceBaseCrud<CondolenciasObituarios, DtoCondolenciasObituarios> CrearServicioCrud()
        {
            return new CondolenciasObituariosService(_uow, _userContext);
        }
    }
}
