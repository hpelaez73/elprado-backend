using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class ClientesService : ServiceBaseCrud<Clientes, DtoClientes>
    {
        public ClientesService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
            _repositoryCrud = _uow.Clientes;
        }
    }
}
