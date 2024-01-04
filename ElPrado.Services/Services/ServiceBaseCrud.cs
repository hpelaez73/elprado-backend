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
        protected RepositoryBaseCrud<TEntidad, TDto> repositoryCrud => (repository as RepositoryBaseCrud<TEntidad, TDto>)!;

        public ServiceBaseCrud(Transaccion? transaccion) : base(transaccion)
        {            
        }

        protected override RepositoryBase CrearRepositorio()
        {
            return CrearRepositorioCrud();
        }

        protected virtual RepositoryBaseCrud<TEntidad, TDto> CrearRepositorioCrud()
        {
            return new(Transaccion);
        }

        public TDto? Visualizar(int id)
        {
            return repositoryCrud.Visualizar(id);
        }
    }
}
