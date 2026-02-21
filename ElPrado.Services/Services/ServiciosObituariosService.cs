using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Mappers;

namespace ElPrado.Services.Services
{
    public class ServiciosObituariosService : ServiceBaseCrud<ServiciosObituarios, DtoServiciosObituarios>
    {
        public ServiciosObituariosService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override IMapper<ServiciosObituarios, DtoServiciosObituarios> CrearMapper()
        {
            return new ServiciosObituariosMapper();
        }

        protected override RepositoryBaseCrud<ServiciosObituarios, DtoServiciosObituarios> CrearRepository()
        {
            return _uow.ServiciosObituarios;
        }
    }
}