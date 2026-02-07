using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Mappers
{
    public interface IMapper<TEntidad, TDto>
        where TDto : DtoBase
        where TEntidad : Entidades, new()
    {
        TEntidad MapToEntity(TDto dto);
        void ApplyToEntity(TEntidad entidad, TDto dto);
    }
}
