using ElPrado.Core;
using ElPrado.Data;
using ElPrado.Data.Models;
using ElPrado.Data.Repositories;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Services
{
    public class ServiceBaseCrud<TEntidad, TDto> : ServiceBase
        where TEntidad : Entidades
        where TDto : DtoBase
    {
        protected RepositoryBaseCrud<TEntidad, TDto>? _repositoryCrud;

        public ServiceBaseCrud(IUnitOfWork unitOfWork, IUserContextService userContext) : base(unitOfWork, userContext)
        {
        }

        public TDto? Visualizar(int id)
        {
            return _repositoryCrud?.Visualizar(id);
        }

        public Resultados<TDto> Actualizar(TDto dto)
        {
            throw new NotImplementedException();
        }

        public ApiResponse<IEnumerable<dynamic>> Listado(DtoOpcionesListados opcionesListado)
        {
            return (_repositoryCrud != null) ? _repositoryCrud.Listado(opcionesListado) : new ApiResponseListado<IEnumerable<dynamic>>();
        }

    }
}
