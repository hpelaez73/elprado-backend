using ElPrado.Data.Interfaces;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;

namespace ElPrado.WebApi.Controllers
{
    public class SuscripcionesObituariosController : ControladorBaseCrud<SuscripcionesObituarios, DtoSuscripcionesObituarios>
    {
        public SuscripcionesObituariosController(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override ServiceBaseCrud<SuscripcionesObituarios, DtoSuscripcionesObituarios> CrearServicioCrud()
        {
            return new SuscripcionesObituariosService(_uow, _userContext);
        }
    }
}