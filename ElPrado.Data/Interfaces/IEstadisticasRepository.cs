using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Interfaces
{
    public interface IEstadisticasRepository
    {
        DtoEstadisticasFila? Resumen(DateTime periodo, string producto);
        List<DtoEstadisticasFila> Evolucion(DateTime desde, DateTime hasta, string producto);
        List<DtoEstadisticasCobertura> Cobertura(DateTime desde, DateTime hasta);
    }
}
