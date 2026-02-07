using ElPrado.Core.Utils;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Mappers
{
    public class ClientesMapper : MapperBase<Clientes, DtoClientes>
    {
        public override void ApplyToEntity(Clientes entidad, DtoClientes dto)
        {
            if (dto == null || entidad == null) return;

            entidad.CodCliente = dto.CodCliente;
            entidad.Nombre = dto.Nombre.Trim();
            entidad.TipoDocumento = DbUtils.NormalizeText(dto.TipoDocumento);
            entidad.NroDocumento = dto.NroDocumento;
            entidad.Cuit = dto.Cuit;
            entidad.CodCatIva = dto.CodCatIva;

            // Mapeo de teléfonos / email: el modelo tiene menos campos que el DTO,
            // asignamos los más relevantes (ajusta si prefieres otra correspondencia).
            entidad.Telefono = DbUtils.NormalizeText(dto.Telefono1);
            entidad.TelefonoMovil = DbUtils.NormalizeText(dto.Telefono2);
            entidad.Email = DbUtils.NormalizeText(dto.Email1);

        }

        public override Clientes MapToEntity(DtoClientes dto) => base.MapToEntity(dto);
    }
}