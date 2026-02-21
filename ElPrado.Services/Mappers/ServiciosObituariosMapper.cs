using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Mappers
{
    public class ServiciosObituariosMapper : MapperBase<ServiciosObituarios, DtoServiciosObituarios>
    {
        public ServiciosObituariosMapper()
        {
        }

        public override void ApplyToEntity(ServiciosObituarios entidad, DtoServiciosObituarios dto)
        {
            if (dto == null || entidad == null) return;
            entidad.CodServicio = dto.CodServicio;
            if (dto.CodObituario != 0) entidad.CodObituario = dto.CodObituario;
            entidad.CodTipoServicio = dto.CodTipoServicio;
            entidad.Lugar = dto.Lugar;
            entidad.FechaHora = dto.FechaHora;
            entidad.UrlMaps = dto.UrlMaps;
        }
    }
}
