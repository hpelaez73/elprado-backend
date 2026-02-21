using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;

namespace ElPrado.WebApi.Controllers
{
    public class ServiciosObituariosController : ControladorBaseCrud<ServiciosObituarios, DtoServiciosObituarios>
    {
        public ServiciosObituariosController(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override ServiceBaseCrud<ServiciosObituarios, DtoServiciosObituarios> CrearServicioCrud()
        {
            return new ServiciosObituariosService(_uow, _userContext);
        }
    }
}
