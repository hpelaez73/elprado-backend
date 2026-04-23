using ElPrado.Data.Interfaces;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Mappers;

namespace ElPrado.Services.Services
{
    public class ClientesService : ServiceBaseCrud<Clientes, DtoClientes>
    {
        public ClientesService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override IMapper<Clientes, DtoClientes> CrearMapper()
        {
            return new ClientesMapper();
        }
        protected override RepositoryBaseCrud<Clientes, DtoClientes> CrearRepository()
        {
            return _uow.Clientes;
        }
    }
}