using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Mappers
{
    public class MediosCobrosMapper : MapperBase<MediosCobros, DtoMediosCobros>
    {
        public override void ApplyToEntity(MediosCobros entidad, DtoMediosCobros dto)
        {
            if (dto == null || entidad == null) return;

            entidad.CodMedioCobro = dto.CodMedioCobro;
            entidad.Descripcion = dto.Descripcion ?? string.Empty;
        }

        public override MediosCobros MapToEntity(DtoMediosCobros dto) => base.MapToEntity(dto);
    }
}
