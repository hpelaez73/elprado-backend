using Dapper;
using ElPrado.Dto.Dtos;

namespace ElPrado.Data.Repositories 
{
    public class ServiciosModelosRepository : RepositoryBase
    {
        public ServiciosModelosRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public List<DtoServiciosHabilitados> ServiciosPropuesta(int codPropuesta)
        {
            string sql = "SELECT * FROM GET_SERVICIOS_PROPUESTA(@codPropuesta)";
            return _connection.Query<DtoServiciosHabilitados>(sql, new { codPropuesta }, _transaction).ToList();
        }

        public List<DtoServiciosUtilizados> ServiciosUtilizadosPropuesta(int codPropuesta)
        {
            string sql = @" SELECT DISTINCT C.MODELO || ' - ' || C.SERVICIO AS SERVICIO, C.SERVICIOS_REALIZADOS, C.SERVICIOS_PENDIENTES, C.COD_SERVICIO_MODELO
                            FROM CONSULTA_SERVICIOS_PROP(@codPropuesta, 0) C
                            WHERE C.LIMITE_SERVICIOS > 0 AND COALESCE(C.VIGENCIA_HASTA, CURRENT_DATE) >= CURRENT_DATE";
            return _connection.Query<DtoServiciosUtilizados>(sql, new { codPropuesta }, _transaction).ToList();
        }

        public List<DtoServiciosBeneficiarios> BeneficiariosPropuesta(int codPropuesta)
        {
            string sql = "SELECT * FROM GET_BENEFICIARIOS_PROPUESTA(@codPropuesta)";
            return _connection.Query<DtoServiciosBeneficiarios>(sql, new { codPropuesta }, _transaction).ToList();
        }
    }
}
