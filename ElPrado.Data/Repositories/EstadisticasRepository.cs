using Dapper;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public sealed class EstadisticasRepository : RepositoryBase
    {
        public EstadisticasRepository(DbContext dbContext) : base(dbContext) { }

        public DtoEstadisticasFila? Resumen(DateTime periodo, string producto) =>
            _connection.QuerySingleOrDefault<DtoEstadisticasFila>(
                "SELECT * FROM RESUMEN_ESTADISTICAS_PARCELAS(@periodo, @producto)",
                new { periodo, producto }, _transaction);

        public List<DtoEstadisticasFila> Evolucion(DateTime desde, DateTime hasta, string producto) =>
            _connection.Query<DtoEstadisticasFila>(
                "SELECT * FROM EVOLUCION_ESTADISTICAS_PARCELAS(@desde, @hasta, @producto)",
                new { desde, hasta, producto }, _transaction).ToList();

        public List<DtoEstadisticasCobertura> Cobertura(DateTime desde, DateTime hasta) =>
            _connection.Query<DtoEstadisticasCobertura>(
                "SELECT * FROM COBERTURA_ESTADISTICAS_PARCELAS(@desde, @hasta)",
                new { desde, hasta }, _transaction).ToList();
    }
}
