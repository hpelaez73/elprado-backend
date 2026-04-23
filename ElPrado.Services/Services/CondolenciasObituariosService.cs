using ElPrado.Core;
using ElPrado.Data.Interfaces;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;
using ElPrado.Services.Mappers;

namespace ElPrado.Services.Services
{
    public class CondolenciasObituariosService : ServiceBaseCrud<CondolenciasObituarios, DtoCondolenciasObituarios>
    {
        public CondolenciasObituariosService(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        protected override IMapper<CondolenciasObituarios, DtoCondolenciasObituarios> CrearMapper()
        {
            return new CondolenciasObituariosMapper();
        }

        protected override RepositoryBaseCrud<CondolenciasObituarios, DtoCondolenciasObituarios> CrearRepository()
        {
            return _uow.CondolenciasObituarios;
        }

        protected override Resultados ValidarAgregar(DtoCondolenciasObituarios dto)
        {
            Resultados resultado = new();

            Obituarios obituario = _uow.Obituarios.Buscar(dto.CodObituario);
            if (!obituario.PermitirComentarios)
            {
                resultado.Agregar("El obituario no permite condolencias");
            }
            dto.Aprobado = !obituario.ModerarCondolencias;
            dto.Fecha = DateTime.Now;

            return resultado;
        }
    }
}
