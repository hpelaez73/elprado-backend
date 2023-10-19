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
        protected RepositoryBaseCrud<TEntidad, TDto> repository;

        public ServiceBaseCrud(Transaccion? transaccion) : base(transaccion)
        {
            repository = CrearRepositorio();
        }

        protected virtual RepositoryBaseCrud<TEntidad, TDto> CrearRepositorio()
        {
            return new(transaccion);
        }

        public TDto? Visualizar(int id)
        {
            return repository.Visualizar(id);
        }
    }
}
