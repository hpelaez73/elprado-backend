using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Mappers
{
    public static class LogginMapper
    {
        public static DtoLogin? MapToDto(Usuarios? entidad)
        {
            if (entidad == null) return null;

            return new DtoLogin()
            {
                CodUsuario = entidad.CodUsuario,
                Nombre = entidad.Nombre
            };
        }
        public static DtoLogin? MapToDto(Clientes? entidad, Propuestas propuesta)
        {
            if (entidad == null) return null;

            return new DtoLogin()
            {
                CodCliente = entidad.CodCliente,
                Nombre = entidad.Nombre,
                CodPropuesta = propuesta.CodPropuesta,
                Propuesta = propuesta.Legajo
            };
        }
    }
}
