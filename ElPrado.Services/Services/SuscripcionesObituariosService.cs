using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Mappers;

namespace ElPrado.Services.Services
{
    public class SuscripcionesObituariosService : ServiceBaseCrud<SuscripcionesObituarios, DtoSuscripcionesObituarios>
    {
        public SuscripcionesObituariosService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override IMapper<SuscripcionesObituarios, DtoSuscripcionesObituarios> CrearMapper()
        {
            return new SuscripcionesObituariosMapper();
        }

        protected override RepositoryBaseCrud<SuscripcionesObituarios, DtoSuscripcionesObituarios> CrearRepository()
        {
            return _uow.SuscripcionesObituarios;
        }
    }
}