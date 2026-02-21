using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Mappers
{
    public class SuscripcionesObituariosMapper : MapperBase<SuscripcionesObituarios, DtoSuscripcionesObituarios>
    {
        public override void ApplyToEntity(SuscripcionesObituarios entidad, DtoSuscripcionesObituarios dto)
        {
            if (dto == null || entidad == null) return;

            entidad.CodSuscripcion = dto.CodSuscripcion;
            if (dto.CodObituario != 0) entidad.CodObituario = dto.CodObituario;
            entidad.Nombre = dto.Nombre;
            if (dto.FechaAlta != DateTime.MinValue) entidad.FechaAlta = dto.FechaAlta;
            entidad.FechaBaja = dto.FechaBaja;
            entidad.Email = dto.Email;
            entidad.Telefono = dto.Telefono;
            entidad.Aniversarios = dto.Aniversarios;
            entidad.Eventos = dto.Eventos;
            entidad.NuevosContenidos = dto.NuevosContenidos;
        }

        public override SuscripcionesObituarios MapToEntity(DtoSuscripcionesObituarios dto) => base.MapToEntity(dto);
    }
}