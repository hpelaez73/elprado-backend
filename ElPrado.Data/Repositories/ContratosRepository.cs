using Dapper;
using ElPrado.Data.Models;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories
{
    public class ContratosRepository : RepositoryBaseEntidad<Contratos>
    {
        public ContratosRepository(Transaccion transaccion) : base(transaccion)
        {
        }

        public List<DtoContratosPropuestas> BuscarContratos(int codPropuesta, bool incluirBaja)
        {
            string sql = "SELECT * FROM CONSULTA_RESUMEN_CONTRATOS (@codPropuesta, @incluirBaja) C ORDER BY C.FECHA DESC, C.TIPO_CONTRATO";
            return connection.Query<DtoContratosPropuestas>(sql, new { codPropuesta, incluirBaja }, transaction).ToList();
        }
    }
}
