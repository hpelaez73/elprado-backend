using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Mappers
{
    public class ComentariosObituariosMapper : MapperBase<ComentariosObituarios, DtoComentariosObituarios>
    {
        public override void ApplyToEntity(ComentariosObituarios entidad, DtoComentariosObituarios dto)
        {
            if (dto == null || entidad == null) return;

            entidad.CodComentario = dto.CodComentario;
            if (dto.CodObituario != 0) entidad.CodObituario = dto.CodObituario;
            entidad.Autor = dto.Autor;
            entidad.Texto = dto.Texto;
            if (dto.Fecha != DateTime.MinValue) entidad.Fecha = dto.Fecha;
            entidad.Aprobado = dto.Aprobado;
            entidad.CodComentarioPadre = dto.CodComentarioPadre;
        }

        public override ComentariosObituarios MapToEntity(DtoComentariosObituarios dto) => base.MapToEntity(dto);
    }
}
