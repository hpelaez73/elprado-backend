using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class DomiciliosRepository : RepositoryBaseCrud<Domicilios, DtoDomicilos>
    {
        public DomiciliosRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public override DtoDomicilos? Visualizar(int id)
        {
            string sql = "SELECT * FROM DOMICILIO_SIMPLE(@id)";
            return connection.QuerySingleOrDefault<DtoDomicilos>(sql, new { id }, transaction);
        }
    }
}
