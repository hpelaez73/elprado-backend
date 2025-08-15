using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class ContratosRepository : RepositoryBaseEntidad<Contratos>
    {
        public ContratosRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public List<DtoContratosPropuestas> BuscarContratos(int codPropuesta, bool incluirBaja)
        {
            string sql = "SELECT * FROM CONSULTA_RESUMEN_CONTRATOS (@codPropuesta, @incluirBaja) C ORDER BY C.FECHA DESC, C.TIPO_CONTRATO";
            return _connection.Query<DtoContratosPropuestas>(sql, new { codPropuesta, incluirBaja }, _transaction).ToList();
        }
    }
}
