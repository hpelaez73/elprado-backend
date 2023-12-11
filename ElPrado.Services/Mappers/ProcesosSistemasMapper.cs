using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Services.Mappers
{
    public static class ProcesosSistemasMapper
    {
        public static ProcesosSistemas MapToEntidad(DtoProcesosSistemas dto)
        {
            return new ProcesosSistemas()
            {
                Proceso = dto.Proceso,
                Nombre = dto.Nombre,
                Categoria = dto.EsSeccionClientes ? "Web cliente" : "Web usuario",
                ModuloWeb = true
            };
        }
    }
}
