using ElPrado.Core.Utils;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Mappers
{
    public class UsuariosMapper : MapperBase<Usuarios, DtoUsuarios>
    {
        public override void ApplyToEntity(Usuarios entidad, DtoUsuarios dto)
        {
            if (dto == null || entidad == null) return;

            entidad.CodUsuario = dto.CodUsuario;
            entidad.Alias = dto.Alias;
            entidad.Nombre = DbUtils.NormalizeText(dto.Nombre);

            // No sobrescribir la clave si viene vacía (útil en actualizaciones)
            if (!string.IsNullOrWhiteSpace(dto.ClaveAcceso))
            {
                entidad.ClaveAcceso = dto.ClaveAcceso;
            }

            entidad.EsAdmin = dto.EsAdmin;
            entidad.SuperUsuario = dto.SuperUsuario;
            entidad.PuedeAutorizar = dto.PuedeAutorizar;
            entidad.CodCliente = dto.CodCliente;
            entidad.CodEmpleado = dto.CodEmpleado;
        }

        public override Usuarios MapToEntity(DtoUsuarios dto) => base.MapToEntity(dto);
    }
}
