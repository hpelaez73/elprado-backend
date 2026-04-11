using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Mappers
{
    public class ObituariosMapper : MapperBase<Obituarios, DtoObituarios>
    {
        public override void ApplyToEntity(Obituarios entidad, DtoObituarios dto)
        {
            if (dto == null || entidad == null) return;

            entidad.CodObituario = dto.CodObituario;
            entidad.Nombre = dto.Nombre;
            entidad.FechaNacimiento = dto.FechaNacimiento?.ToDateTime(TimeOnly.MinValue);
            entidad.FechaFallecimiento = dto.FechaFallecimiento?.ToDateTime(TimeOnly.MinValue);
            entidad.UrlImagen = dto.UrlImagen;
            entidad.Biografia = dto.Biografia;
            entidad.PermitirCondolencias = dto.PermitirCondolencias;
            entidad.ModerarCondolencias = dto.ModerarCondolencias;
            entidad.PermitirComentarios = dto.PermitirComentarios;
            entidad.ModerarComentarios = dto.ModerarComentarios;
        }

        public override Obituarios MapToEntity(DtoObituarios dto) => base.MapToEntity(dto);
    }
}
