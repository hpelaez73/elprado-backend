using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Mappers
{
    public abstract class MapperBase<TEntidad, TDto> : IMapper<TEntidad, TDto>
        where TDto : DtoBase
        where TEntidad : Entidades, new()
    {
        protected MapperBase()
        {
        }

        public virtual TEntidad MapToEntity(TDto dto)
        {
            TEntidad entidad = new();
            ApplyToEntity(entidad, dto);
            return entidad;
        }

        public abstract void ApplyToEntity(TEntidad entidad, TDto dto);
    }
}
