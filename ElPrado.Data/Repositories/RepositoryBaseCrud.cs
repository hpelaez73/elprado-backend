using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class RepositoryBaseCrud<TEntidad, TDto> : RepositoryBaseEntidad<TEntidad>
        where TEntidad : Entidades
        where TDto : DtoBase
    {
        public RepositoryBaseCrud(DbContext dbContext) : base(dbContext)
        {
        }

        public virtual TDto? Visualizar(int id)
        {
            return null;
        }

    }
}
