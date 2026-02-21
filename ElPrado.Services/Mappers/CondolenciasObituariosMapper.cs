using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Mappers
{
    public class CondolenciasObituariosMapper : MapperBase<CondolenciasObituarios, DtoCondolenciasObituarios>
    {
        public override void ApplyToEntity(CondolenciasObituarios entidad, DtoCondolenciasObituarios dto)
        {
            if (dto == null || entidad == null) return;

            entidad.CodCondolencia = dto.CodCondolencia;
            if (dto.CodObituario != 0) entidad.CodObituario = dto.CodObituario;
            entidad.Autor = dto.Autor;
            entidad.Mensaje = dto.Mensaje;
            if (dto.Fecha != DateTime.MinValue) entidad.Fecha = dto.Fecha;
            entidad.Aprobado = dto.Aprobado;
        }

        public override CondolenciasObituarios MapToEntity(DtoCondolenciasObituarios dto) => base.MapToEntity(dto);
    }
}
