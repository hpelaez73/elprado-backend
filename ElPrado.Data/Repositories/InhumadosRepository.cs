using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class InhumadosRepository : RepositoryBaseEntidad<Inhumados>
    {
        public InhumadosRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public DtoInhumacion? BuscarInhumacion(int id)
        {
            string sql = "SELECT * FROM GET_DATOS_INHUMADO(@id)";
            return _connection.QuerySingleOrDefault<DtoInhumacion>(sql, new { id }, _transaction);
        }

        public List<DtoInhumadosPropuestas> BuscarInhumados(int codPropuesta, int codParcela)
        {
            string sql = "SELECT * FROM GET_DATOS_INHUMADOS(@codPropuesta, @codParcela)";
            return _connection.Query<DtoInhumadosPropuestas>(sql, new { codPropuesta, codParcela }, _transaction).ToList();
        }
    }
}
