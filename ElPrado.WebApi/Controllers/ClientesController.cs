using ElPrado.Data.Interfaces;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;
using ElPrado.Services;
using ElPrado.Services.Services;

namespace ElPrado.WebApi.Controllers
{
    public class ClientesController : ControladorBaseCrud<Clientes, DtoClientes>
    {
        public ClientesController(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        { 
        }

        protected override ServiceBaseCrud<Clientes, DtoClientes> CrearServicio()
        {
            return new ClientesService(_uow, _userContext);
        }
    }
}
