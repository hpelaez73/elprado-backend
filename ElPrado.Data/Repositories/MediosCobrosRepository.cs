using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class MediosCobrosRepository : RepositoryBaseCrud<MediosCobros, DtoMediosCobros>
    {
        public MediosCobrosRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public override DtoMediosCobros? Visualizar(int id)
        {
            string sql = "SELECT * FROM GET_DATOS_MEDIO_COBRO(@id)";
            return _connection.QuerySingleOrDefault<DtoMediosCobros>(sql, new { id }, _transaction);
        }

        public List<DtoHistorialCobradores> BuscarHistorialCobradores(int? codCredito, int? codConfiguracion)
        {
            string sql = "SELECT * FROM GET_HISTORIAL_COBRADORES(@codCredito, @codConfiguracion)";
            return _connection.Query<DtoHistorialCobradores>(sql, new { codCredito, codConfiguracion }, _transaction).ToList();
        }

    }
}
