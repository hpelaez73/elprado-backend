using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class CondolenciasObituariosRepository : RepositoryBaseCrud<CondolenciasObituarios, DtoCondolenciasObituarios>
    {
        public CondolenciasObituariosRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public override DtoCondolenciasObituarios? Visualizar(int id)
        {
            string sql = "SELECT * FROM CONDOLENCIAS_OBITUARIOS WHERE COD_CONDOLENCIA = @id";
            return _connection.QueryFirstOrDefault<DtoCondolenciasObituarios>(sql, new { id });
        }

        public List<DtoCondolenciasObituarios> BuscarPorObituario(int id)
        {
            string sql = "SELECT * FROM CONDOLENCIAS_OBITUARIOS C WHERE C.COD_OBITUARIO = @id";
            return _connection.Query<DtoCondolenciasObituarios>(sql, new { id }, _transaction).ToList();
        }

    }
}
